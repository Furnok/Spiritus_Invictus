using Sirenix.OdinInspector;
using UnityEngine;

public class S_TargetsDetectorDebug : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("General")]
    [SerializeField] private Color gizmoColor;

    [TabGroup("Settings")]
    [SerializeField] private bool drawGizmos;

    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] private SphereCollider detectionCollider;

    private void OnDrawGizmos()
    {
        if (!enabled || !drawGizmos || detectionCollider == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(gameObject.transform.position, detectionCollider.radius);
    }
}