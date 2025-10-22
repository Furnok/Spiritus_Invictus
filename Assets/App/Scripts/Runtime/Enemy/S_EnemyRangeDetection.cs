using UnityEngine;
using UnityEngine.Events;

public class S_EnemyRangeDetection : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [S_TagName] private string playerTag;
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] float detectionRange;
    [SerializeField] float aggroRange;

    [Header("References")]
    [SerializeField] SphereCollider detectionCollider;
    [SerializeField] S_Entity S_Entity;
    [HideInInspector] public UnityEvent<GameObject> onTargetDetected;

    private GameObject targetDetected = null;

    private void OnEnable()
    {
        S_Entity.onGetHit.AddListener(AggroPlayer);
    }
    private void OnDisable()
    {
        S_Entity.onGetHit.RemoveListener(AggroPlayer);
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
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            targetDetected = null;
            onTargetDetected.Invoke(targetDetected);
        }
    }

    private void AggroPlayer()
    {
       Collider[] hit =  Physics.OverlapSphere(transform.position, aggroRange, playerLayerMask);
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
}