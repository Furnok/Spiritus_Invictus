using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerHeadLookAtIK : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_TargetPosition rsoTargetPosition;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_EnemyHead ssoEnemyHead;

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || !rsoPlayerIsTargeting.Value || rsoTargetPosition.Value == null) return;

        animator.SetLookAtWeight(
            ssoEnemyHead.Value.weight,
            ssoEnemyHead.Value.bodyWeight,
            ssoEnemyHead.Value.headWeight,
            ssoEnemyHead.Value.eyesWeight,
            ssoEnemyHead.Value.clampWeight
        );

        animator.SetLookAtPosition(rsoTargetPosition.Value);
    }
}