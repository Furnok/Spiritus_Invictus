using UnityEngine;

public class S_TargetsDetectorDebug : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color gizmoColor;
    [SerializeField] private bool drawGizmos;

    [Header("References")]
    [SerializeField] private SphereCollider detectionCollider;

    private void OnDrawGizmos()
    {
        if (!enabled || !drawGizmos || detectionCollider == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(gameObject.transform.position/*_detectionCollider.center*/, detectionCollider.radius);
    }
}