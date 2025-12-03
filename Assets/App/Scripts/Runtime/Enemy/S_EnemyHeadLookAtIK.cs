using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyHeadLookAtIK : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Weight")]
    [SerializeField, Range(0f, 1f)] private float weight = 1f;

    [TabGroup("Settings")]
    [SerializeField, Range(0f, 1f)] private float bodyWeight = 0f;

    [TabGroup("Settings")]
    [SerializeField, Range(0f, 1f)] private float headWeight = 1f;

    [TabGroup("Settings")]
    [SerializeField, Range(0f, 1f)] private float eyesWeight = 0f;

    [TabGroup("Settings")]
    [SerializeField, Range(0f, 1f)] private float clampWeight = 0.5f;

    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    private GameObject target = null;

    public void SetTarget(GameObject targetPos)
    {
        target = targetPos;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || target == null) return;

        animator.SetLookAtWeight(
            weight,
            bodyWeight,
            headWeight,
            eyesWeight,
            clampWeight
        );

        animator.SetLookAtPosition(target.GetComponent<S_LookAt>().GetAimPoint());
    }
}