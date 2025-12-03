using Sirenix.OdinInspector;
using UnityEngine;

public class S_HeadLookAtIK : MonoBehaviour
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

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_TargetPosition rsoTargetPosition;

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || !rsoPlayerIsTargeting.Value || rsoTargetPosition.Value == null) return;

        animator.SetLookAtWeight(
            weight,
            bodyWeight,
            headWeight,
            eyesWeight,
            clampWeight
        );

        animator.SetLookAtPosition(rsoTargetPosition.Value);
    }
}