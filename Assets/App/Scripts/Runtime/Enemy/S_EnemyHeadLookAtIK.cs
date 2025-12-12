using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyHeadLookAtIK : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_EnemyHead ssoEnemyHead;

    private GameObject target = null;
    private bool isDead = false;

    public void SetTarget(GameObject targetPos)
    {
        target = targetPos;
    }

    public void IsDead(bool value)
    {
        isDead = value;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || target == null || isDead) return;

        animator.SetLookAtWeight(
            ssoEnemyHead.Value.weight,
            ssoEnemyHead.Value.bodyWeight,
            ssoEnemyHead.Value.headWeight,
            ssoEnemyHead.Value.eyesWeight,
            ssoEnemyHead.Value.clampWeight
        );

        animator.SetLookAtPosition(target.GetComponent<S_LookAt>().GetAimPoint());
    }
}