using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public class S_EnemyAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Input")]
    [HideInInspector] public UnityEvent<float> UpdateTimerAnimation;
    [SerializeField] private RSE_OnCallGetTimerAnimByName rseOnCallGetTimerAnimByName;

    private float timer = 0;

    private void OnEnable()
    {
        rseOnCallGetTimerAnimByName.Event += GetTotalLengthOfSubStateMachine;
    }

    private void OnDisable()
    {
        rseOnCallGetTimerAnimByName.Event -= GetTotalLengthOfSubStateMachine;
    }

    private void GetTotalLengthOfSubStateMachine(string subStateMachineName)
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

            AnimatorStateMachine targetSM = FindSubStateMachineByName(rootSM, subStateMachineName);
            if (targetSM == null)
            {
                Debug.LogWarning($"Sub-state machine '{subStateMachineName}' not found in layer '{layer.name}'.");
                continue;
            }

            HashSet<Motion> motions = new HashSet<Motion>();
            CollectMotions(targetSM, motions);

            float totalLength = 0f;

            foreach (var motion in motions)
            {
                totalLength += GetMotionLength(motion);
            }

            timer = totalLength;
            UpdateTimerAnimation.Invoke(timer);
        }
    }

    private AnimatorStateMachine FindSubStateMachineByName(AnimatorStateMachine root, string name)
    {
        foreach (var child in root.stateMachines)
        {
            if (child.stateMachine.name == name)
                return child.stateMachine;
        }

        return null;
    }

    private void CollectMotions(AnimatorStateMachine stateMachine, HashSet<Motion> collected)
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

    private float GetMotionLength(Motion motion)
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

    private float EstimateBlendTreeLength(BlendTree tree)
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