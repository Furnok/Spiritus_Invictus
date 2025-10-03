using UnityEngine;

public class S_TargetDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [S_TagName] private string tagEnemy;

    [Header("References")]
    [SerializeField] private SphereCollider sphereCollider;

    [Header("Output")]
    [SerializeField] private RSE_OnEnemyEnterTargetingRange rseOnEnemyEnterTargetingRange;
    [SerializeField] private RSE_OnEnemyExitTargetingRange rseOnEnemyExitTargetingRange;
    [SerializeField] private SSO_PlayerTargetRangeRadius ssoPlayerTargetRangeRadius;

    private void Awake()
    {
        sphereCollider.radius = ssoPlayerTargetRangeRadius.Value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagEnemy))
        {
            if (rseOnEnemyEnterTargetingRange != null)
            {
                rseOnEnemyEnterTargetingRange.Call(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagEnemy))
        {
            if (rseOnEnemyExitTargetingRange != null)
            {
                rseOnEnemyExitTargetingRange.Call(other.gameObject);
            }
        }
    }
}