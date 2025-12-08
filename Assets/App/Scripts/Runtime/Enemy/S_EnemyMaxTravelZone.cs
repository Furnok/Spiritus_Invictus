using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyMaxTravelZone : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filters")]
    [SerializeField, S_TagName] private string playerTag;

    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] private BoxCollider box;

    [TabGroup("References")]
    [Title("Script")]
    [SerializeField] private S_Enemy enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            enemy.SetTargetInMaxTravelZone(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            enemy.SetTargetInMaxTravelZone(null);
            enemy.SetTarget(null);
        }
    }

    public void Setup(SSO_EnemyData enemyData)
    {
        box.size = new Vector3(enemyData.Value.detectionAggroRangeMax, enemyData.Value.detectionAggroRangeMax, enemyData.Value.detectionAggroRangeMax);
    }
}