using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class S_EnemyDetectionRange : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filters")]
    [SerializeField, S_TagName] private string playerTag;

    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] private SphereCollider detectionCollider;

    [TabGroup("References")]
    [Title("Script")]
    [SerializeField] private S_Enemy enemy;

    [HideInInspector] public UnityEvent<GameObject> onTargetDetected = null;

    private GameObject targetDetected = null;

    private float detectionRange = 0;

    private void Start()
    {
        detectionCollider.radius = detectionRange;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            targetDetected = other.gameObject;
            onTargetDetected.Invoke(targetDetected);
        }
    }

    public void Setup(SSO_EnemyData enemyData)
    {
        detectionRange = enemyData.Value.detectionRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}