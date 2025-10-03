using UnityEngine;
using UnityEngine.Events;

public class S_EnemyRangeDetection : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [S_TagName] private string playerTag;

    [HideInInspector] public UnityEvent<GameObject> onTargetDetected;

    private GameObject targetDetected = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            targetDetected = other.gameObject;
            onTargetDetected.Invoke(targetDetected);
        }
        
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