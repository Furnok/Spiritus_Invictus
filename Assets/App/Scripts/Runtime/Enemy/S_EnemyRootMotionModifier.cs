using Sirenix.OdinInspector;
using UnityEngine;

public class S_EnemyRootMotionModifier : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Filters")]
    [SerializeField, S_TagName] private string tagPlayer;

    [TabGroup("References")]
    [SerializeField, S_TagName] private string tagObstacle;

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

    private float rootMotionMultiplier = 1f;
    private float distanceMin = 0;

    private void OnAnimatorMove()
    {
        if (isPause.Value) return;

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

    public void Setup(float newMultiplicator, float newDistanceMin)
    {
        rootMotionMultiplier = newMultiplicator;
        distanceMin = newDistanceMin;
    }

    private bool CanMove(Vector3 delta)
    {
        RaycastHit hit;
        if (Physics.Raycast(body.transform.position, delta.normalized, out hit, distanceMin))
        {
            if (hit.collider.CompareTag(tagPlayer) || hit.collider.CompareTag(tagObstacle)) return false;
        }

        return true;
    }
}