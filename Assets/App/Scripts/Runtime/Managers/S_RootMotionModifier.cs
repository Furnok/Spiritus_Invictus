using Sirenix.OdinInspector;
using UnityEngine;

public class S_RootMotionModifier : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Filter")]
    [SerializeField, S_TagName] private string tagPlayer;

    [TabGroup("Settings")]
    [Title("Move Multiplicator")]
    [SerializeField] private float rootMotionMultiplier = 2f;

    [TabGroup("References")]
    [Title("Animator")]
    [SerializeField] private Animator animator;

    [TabGroup("References")]
    [Title("Rigidbody")]
    [SerializeField] private Rigidbody rb;

    [TabGroup("References")]
    [Title("Body")]
    [SerializeField] private GameObject body;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_GameInPause isPause;

    private void OnAnimatorMove()
    {
        if (isPause.Value)
            return;

        Vector3 delta = animator.deltaPosition * rootMotionMultiplier;
        Quaternion deltaRot = animator.deltaRotation;

        if (rb != null)
        {
            if (!CanMove(delta)) return;

            transform.position += delta;
            transform.rotation *= deltaRot;
        }
        else
        {
            transform.position += delta;
            transform.rotation *= deltaRot;
        }
    }

    private bool CanMove(Vector3 delta)
    {
        RaycastHit hit;
        if (Physics.Raycast(body.transform.position, delta.normalized, out hit, 1f))
        {
            if (hit.collider.CompareTag(tagPlayer))
            {
                return false;
            }
        }

        return true;
    }
}