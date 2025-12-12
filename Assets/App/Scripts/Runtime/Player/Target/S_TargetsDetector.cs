using Sirenix.OdinInspector;
using UnityEngine;

public class S_TargetDetector : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Filter")]
    [SerializeField, S_TagName] private string tagEnemy;

    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] private SphereCollider sphereCollider;

    [TabGroup("Intputs")]
    [SerializeField] private RSE_OnEnemyTargetDied rseOnEnemyTargetDied;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnEnemyEnterTargetingRange rseOnEnemyEnterTargetingRange;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnEnemyExitTargetingRange rseOnEnemyExitTargetingRange;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerTargetRangeRadius ssoPlayerTargetRangeRadius;

    private void Awake()
    {
        sphereCollider.radius = ssoPlayerTargetRangeRadius.Value;
    }

    private void OnEnable()
    {
        rseOnEnemyTargetDied.action += OnEnemyTargetDied;
    }

    private void OnDisable()
    {
        rseOnEnemyTargetDied.action -= OnEnemyTargetDied;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagEnemy))
        {
            if (rseOnEnemyEnterTargetingRange != null) rseOnEnemyEnterTargetingRange.Call(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagEnemy))
        {
            if (rseOnEnemyExitTargetingRange != null) rseOnEnemyExitTargetingRange.Call(other.gameObject);
        }
    }

    private void OnEnemyTargetDied(GameObject enemyObject)
    {
        if (enemyObject != null) rseOnEnemyExitTargetingRange.Call(enemyObject);
    }
}