using UnityEngine;

public class S_PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [S_AnimationName] private string moveParam;
    [SerializeField] [S_AnimationName] private string speedParam;

    [Header("References")]
    [SerializeField] private Rigidbody rigidbodyPlayer;

    [Header("Input")]
    [SerializeField] private RSO_PlayerPosition rsoPlayerPosition;
    [SerializeField] private RSE_OnPlayerMove rseOnPlayerMove;
    [SerializeField] private RSE_OnNewTargeting rseOnNewTargeting;
    [SerializeField] private RSE_OnPlayerCancelTargeting rseOnPlayerCancelTargeting;

    [Header("Output")]
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;
    [SerializeField] private RSE_OnAnimationFloatValueChange rseOnAnimationFloatValueChange;
    [SerializeField] private RSO_CameraRotation rsoCameraRotation;
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;
    [SerializeField] private RSO_CurrentInputActionMap rsoCurrentInputActionMap;
    [SerializeField] private SSO_PlayerMovementSpeed ssoPlayerMovementSpeed;
    [SerializeField] private SSO_PlayerTurnSpeed ssoPlayerTurnSpeed;
    [SerializeField] private SSO_PlayerTurnSpeedTargeting ssoPlayerTurnSpeedTargeting;
    [SerializeField] private SSO_PlayerStrafeSpeed ssoPlayerStrafeSpeed;

    private Vector2 moveInput = Vector2.zero;
    private bool inputCanceledOrNoInput = true;
    private Quaternion camRotInInputPerformed = Quaternion.identity;
    private Transform target = null;

    private void Awake()
    {
        rsoPlayerPosition.Value = transform.position;

        rigidbodyPlayer.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnDestroy()
    {
        rsoPlayerPosition.Value = Vector3.zero;
    }

    private void OnEnable()
    {
        rsoPlayerPosition.Value = transform.position;
        rseOnPlayerMove.action += Move;
        rseOnNewTargeting.action += ChangeNewTargt;
        rseOnPlayerCancelTargeting.action += CancelTarget;
    }

    private void OnDisable()
    {
        rseOnPlayerMove.action -= Move;
        rseOnNewTargeting.action -= ChangeNewTargt;
        rseOnPlayerCancelTargeting.action -= CancelTarget;
    }

    private void ChangeNewTargt(GameObject newTarget)
    {
        target = newTarget.transform;
    }

    private void CancelTarget(GameObject oldTarget)
    {
        target = null;
    }

    private void Move(Vector2 input)
    {
        moveInput = input;

        if(moveInput != Vector2.zero)
        {
            rseOnAnimationBoolValueChange.Call(moveParam, true);
            inputCanceledOrNoInput = false;
        }
        else
        {
            rseOnAnimationBoolValueChange.Call(moveParam, false);
            inputCanceledOrNoInput = true;
        }
    }

    private void Update()
    {
        if (rsoPlayerIsTargeting.Value && target != null)
        {
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0f; // Ignore the heigth

            if (directionToTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                rigidbodyPlayer.MoveRotation(Quaternion.Slerp(rigidbodyPlayer.rotation, targetRotation, ssoPlayerTurnSpeedTargeting.Value * Time.fixedDeltaTime));
            }

            Vector3 right = Vector3.Cross(Vector3.up, directionToTarget.normalized);
            Vector3 forward = directionToTarget.normalized;

            Vector3 desiredDirection = right * moveInput.x + forward * moveInput.y;
            desiredDirection.Normalize();

            float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);
            Vector3 desiredVelocity = desiredDirection * ssoPlayerStrafeSpeed.Value * inputMagnitude;

            Vector3 velocityTargeting = rigidbodyPlayer.linearVelocity;
            velocityTargeting.x = desiredVelocity.x;
            velocityTargeting.z = desiredVelocity.z;
            rigidbodyPlayer.linearVelocity = velocityTargeting;

            rseOnAnimationFloatValueChange.Call(speedParam, velocityTargeting.magnitude);


            rsoPlayerPosition.Value = transform.position;
            return;
        }
    }

    private void FixedUpdate()
    {
        if (rsoCurrentInputActionMap.Value == S_EnumPlayerInputActionMap.Game)
        {
            if (!rsoPlayerIsTargeting.Value)
            {
                Quaternion camRot;

                if (inputCanceledOrNoInput == true)
                {
                    camRot = rsoCameraRotation ? rsoCameraRotation.Value : Quaternion.identity; //take the rotation of the camera if exist otherwise take the world
                }
                else
                {
                    camRot = camRotInInputPerformed;
                }

                Vector3 camForward = camRot * Vector3.forward;
                camForward.y = 0f; //ignore vertical camera forward
                camForward.Normalize();

                Vector3 camRight = camRot * Vector3.right;
                camRight.y = 0f;
                camRight.Normalize();

                Vector3 desiredDir = camRight * moveInput.x + camForward * moveInput.y; //desired direction in world space from the input and the camera orientation

                if (moveInput != Vector2.zero) //turn character only if there is some input
                {
                    desiredDir.Normalize();
                    Quaternion target = Quaternion.LookRotation(desiredDir, Vector3.up);
                    rigidbodyPlayer.MoveRotation(Quaternion.Slerp(rigidbodyPlayer.rotation, target, ssoPlayerTurnSpeed.Value * Time.fixedDeltaTime));
                }
                else
                {
                    rigidbodyPlayer.angularVelocity = Vector3.zero;
                    desiredDir = Vector3.zero;
                }

                float inputMag = Mathf.Clamp01(moveInput.magnitude);
                Vector3 desiredVel = desiredDir * ssoPlayerMovementSpeed.Value * inputMag;


                Vector3 velocity = rigidbodyPlayer.linearVelocity;
                velocity.x = desiredVel.x;
                velocity.z = desiredVel.z;
                rigidbodyPlayer.linearVelocity = velocity;

                rseOnAnimationFloatValueChange.Call(speedParam, velocity.magnitude);

                rsoPlayerPosition.Value = transform.position;

                if (inputCanceledOrNoInput == true)
                {
                    camRotInInputPerformed = camRot;
                }
            }
        }
        else
        {
            rigidbodyPlayer.linearVelocity = Vector3.zero;

            rseOnAnimationFloatValueChange.Call(speedParam, 0);
        }
    }
}

