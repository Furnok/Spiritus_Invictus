using Sirenix.OdinInspector;
using UnityEngine;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            enemy.SetTarget(other.gameObject);
        }
    }

    public void Setup(SSO_EnemyData enemyData)
    {
        detectionCollider.radius = enemyData.Value.detectionRange;
    }
}