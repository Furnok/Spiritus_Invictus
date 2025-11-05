using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class S_EnemyDetectionRange : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filters")]
    [SerializeField][S_TagName] private string playerTag;

    [TabGroup("Settings")]
    [SerializeField] LayerMask playerLayerMask;

    [TabGroup("References")]
    [SerializeField] private SphereCollider detectionCollider;

    [TabGroup("References")]
    [SerializeField] private S_Enemy enemy;

    [HideInInspector] public UnityEvent<GameObject> onTargetDetected;

    private GameObject targetDetected = null;
    private float detectionRange = 0;
    private float aggroRange = 0;

    private void OnEnable()
    {
        enemy.onGetHit.AddListener(AggroPlayer);
    }

    private void OnDisable()
    {
        enemy.onGetHit.RemoveListener(AggroPlayer);
    }

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
        aggroRange = enemyData.Value.detectionAggroRangeMax;
    }

    private void AggroPlayer()
    {
        Collider[] hit = Physics.OverlapSphere(transform.position, aggroRange, playerLayerMask);
        foreach (Collider c in hit)
        {
            if (c.CompareTag(playerTag))
            {
                targetDetected = c.gameObject;
                onTargetDetected.Invoke(targetDetected);
                detectionCollider.enabled = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}