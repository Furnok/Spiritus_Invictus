using UnityEngine;

public class S_TargetDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [S_TagName] private string tagEnemy;

    [Header("References")]
    [SerializeField] private SphereCollider sphereCollider;

    [Header("Intput")]
    [SerializeField] private RSE_OnEnemyTargetDied rseOnEnemyTargetDied;

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
                Debug.Log("Enemy entered targeting range: " + other.gameObject.name);
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
                Debug.Log("Enemy exit targeting range: " + other.gameObject.name);
            }
        }
    }

    void OnEnable()
    {
        rseOnEnemyTargetDied.action += OnEnemyTargetDied;
    }

    void OnDisable()
    {
        rseOnEnemyTargetDied.action -= OnEnemyTargetDied;
    }

    void OnEnemyTargetDied(GameObject enemyObject)
    {
        if(enemyObject != null)
        {
            rseOnEnemyExitTargetingRange.Call(enemyObject);
            Debug.Log("Enemy target died and exited targeting range: " + enemyObject.name);
        }
    }
}