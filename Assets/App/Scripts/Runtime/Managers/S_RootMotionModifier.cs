using UnityEngine;

public class S_RootMotionModifier : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float rootMotionMultiplier = 2f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    //[Header("Inputs")]

    //[Header("Outputs")]
    [SerializeField] private RSO_GameInPause isPause;

    private void OnAnimatorMove()
    {
        if(isPause.Value) return;

        if (rb != null)
        {
            Vector3 deltaPosition = animator.deltaPosition * rootMotionMultiplier;
            Vector3 velocity = deltaPosition / Time.deltaTime;
            velocity.y = rb.linearVelocity.y;
            rb.linearVelocity = velocity;

            rb.MoveRotation(rb.rotation * animator.deltaRotation);
        }
        else
        {
            transform.position += animator.deltaPosition * rootMotionMultiplier;
            transform.rotation *= animator.deltaRotation;
        }
    }
}