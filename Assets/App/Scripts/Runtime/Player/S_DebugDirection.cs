using UnityEngine;

public class S_DebugDirection : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float length;
    [SerializeField] private float yOffset;
    [SerializeField] private Color color;

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        Gizmos.color = color;

        Vector3 start = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
        Vector3 end = start + transform.forward * length;

        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.1f);
    }
}