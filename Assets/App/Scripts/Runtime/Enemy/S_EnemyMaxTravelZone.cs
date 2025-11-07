using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class S_EnemyMaxTravelZone : MonoBehaviour
{
    [TabGroup("Settings")]
    [SerializeField][S_TagName] private string playerTag;

    [HideInInspector] public UnityEvent<GameObject> onTargetDetected;
    private GameObject targetDetected = null;
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            targetDetected = null;
            onTargetDetected.Invoke(targetDetected);
        }
    }
}