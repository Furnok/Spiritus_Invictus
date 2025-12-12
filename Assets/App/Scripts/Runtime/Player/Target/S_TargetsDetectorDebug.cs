using Sirenix.OdinInspector;
using UnityEngine;

public class S_TargetsDetectorDebug : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] private SphereCollider detectionCollider;

    private void OnDrawGizmos()
    {
        if (!enabled || detectionCollider == null) return;

        Color color = Color.green;
        color.a = 0.05f;

        Gizmos.color = color;
        Gizmos.DrawSphere(gameObject.transform.position, detectionCollider.radius);
    }
}