using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class S_BossDetectionRange : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filters")]
    [SerializeField][S_TagName] private string playerTag;

    [HideInInspector] public UnityEvent<GameObject> onTargetDetected;

    private GameObject target;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Target");
            target = other.gameObject;
            onTargetDetected.Invoke(target);
        }
    }
}