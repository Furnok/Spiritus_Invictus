using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class S_EnemyMaxTravelZone : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filters")]
    [SerializeField][S_TagName] private string playerTag;

    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] private BoxCollider box;

    [HideInInspector] public UnityEvent<GameObject> onTargetDetected;
    private GameObject targetDetected = null;

    public void Setup(SSO_EnemyData enemyData)
    {
        box.size = new Vector3(enemyData.Value.detectionAggroRangeMax, enemyData.Value.detectionAggroRangeMax, enemyData.Value.detectionAggroRangeMax);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            targetDetected = null;
            onTargetDetected.Invoke(targetDetected);
        }
    }
}