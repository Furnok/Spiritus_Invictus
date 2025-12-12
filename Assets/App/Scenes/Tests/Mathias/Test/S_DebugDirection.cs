using Sirenix.OdinInspector;
using UnityEngine;

public class S_DebugDirection : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("General")]
    [SerializeField] private float length;

    [TabGroup("Settings")]
    [SerializeField] private float yOffset;

    [TabGroup("Settings")]
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