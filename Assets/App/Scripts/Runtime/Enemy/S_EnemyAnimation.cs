using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public class S_EnemyAnimation : MonoBehaviour
{
    //[Header("Settings")]
    private float timer;

    [Header("References")]
    [SerializeField] Animator animator;

    [Header("Input")]
    [HideInInspector] public UnityEvent<float> UpdateTimerAnimation;
    [SerializeField] CallGetTimerAnimByName CallGetTimerAnimByName;

    //[Header("Output")]
    //private void Start()
    //{
    //    GetTotalLengthOfSubStateMachine("Combo Attack 1");
    //    Debug.Log(timer);
    //}
    private void OnEnable()
    {
        CallGetTimerAnimByName.Event += GetTotalLengthOfSubStateMachine;
    }
    private void OnDisable()
    {
        CallGetTimerAnimByName.Event -= GetTotalLengthOfSubStateMachine;
    }

    void GetTotalLengthOfSubStateMachine(string subStateMachineName)
    {
        Animator animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("No Animator component found.");
            return;
        }

        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null)
        {
            Debug.LogError("Animator does not use an AnimatorController.");
            return;
        }

        foreach (var layer in controller.layers)
        {
            AnimatorStateMachine rootSM = layer.stateMachine;

            // Try to find the sub-state machine by name
            AnimatorStateMachine targetSM = FindSubStateMachineByName(rootSM, subStateMachineName);
            if (targetSM == null)
            {
                Debug.LogWarning($"Sub-state machine '{subStateMachineName}' not found in layer '{layer.name}'.");
                continue;
            }

            // Collect motions inside the found sub-state machine
            HashSet<Motion> motions = new HashSet<Motion>();
            CollectMotions(targetSM, motions);

            float totalLength = 0f;

            foreach (var motion in motions)
            {
                totalLength += GetMotionLength(motion);
            }
            timer = totalLength;
            UpdateTimerAnimation.Invoke(timer);
            //Debug.Log($"[Layer: {layer.name}] Sub-State Machine: {subStateMachineName} — Total Motion Length: {totalLength:F2} seconds");
        }
    }

    AnimatorStateMachine FindSubStateMachineByName(AnimatorStateMachine root, string name)
    {
        foreach (var child in root.stateMachines)
        {
            if (child.stateMachine.name == name)
                return child.stateMachine;
        }
        return null;
    }

    void CollectMotions(AnimatorStateMachine stateMachine, HashSet<Motion> collected)
    {
        foreach (var state in stateMachine.states)
        {
            if (state.state.motion != null)
            {
                collected.Add(state.state.motion);
            }
        }

        foreach (var sub in stateMachine.stateMachines)
        {
            CollectMotions(sub.stateMachine, collected);
        }
    }

    float GetMotionLength(Motion motion)
    {
        if (motion is AnimationClip clip)
        {
            return clip.length;
        }
        else if (motion is BlendTree tree)
        {
            return EstimateBlendTreeLength(tree);
        }
        return 0f;
    }

    float EstimateBlendTreeLength(BlendTree tree)
    {
        float max = 0f;
        foreach (var child in tree.children)
        {
            if (child.motion is AnimationClip clip)
            {
                max = Mathf.Max(max, clip.length);
            }
            else if (child.motion is BlendTree subTree)
            {
                max = Mathf.Max(max, EstimateBlendTreeLength(subTree));
            }
        }
        return max;
    }
}