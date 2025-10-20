using UnityEngine;

public class S_PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] [S_AnimationName] private string moveParam;
    [SerializeField] [S_AnimationName] private string speedParam;
    [SerializeField, S_AnimationName] string _strafXParam;
    [SerializeField, S_AnimationName] string _strafYParam;

    [Header("References")]
    [SerializeField] private Rigidbody rigidbodyPlayer;
    [SerializeField] RSO_PlayerIsDodging _playerIsDodging;
    [SerializeField] SSO_PlayerStateTransitions _playerStateTransitions;
    [SerializeField] RSO_PlayerCurrentState _playerCurrentState;
    [SerializeField] SSO_PlayerStats _playerStats;

    [Header("Input")]
    [SerializeField] private RSO_PlayerPosition rsoPlayerPosition;
    [SerializeField] RSO_PlayerRotation rsoPlayerRotation;
    [SerializeField] private RSE_OnPlayerMove rseOnPlayerMove;
    [SerializeField] private RSE_OnNewTargeting rseOnNewTargeting;
    [SerializeField] private RSE_OnPlayerCancelTargeting rseOnPlayerCancelTargeting;

    [Header("Output")]
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;
    [SerializeField] private RSE_OnAnimationFloatValueChange rseOnAnimationFloatValueChange;
    [SerializeField] private RSO_CameraRotation rsoCameraRotation;
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;
    [SerializeField] private RSO_CurrentInputActionMap rsoCurrentInputActionMap;
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;

    private Vector2 moveInput = Vector2.zero;
    private bool inputCanceledOrNoInput = true;
    private Quaternion camRotInInputPerformed = Quaternion.identity;
    private Transform target = null;

    private void Awake()
    {
        rsoPlayerPosition.Value = transform.position;
        rsoPlayerRotation.Value = transform.rotation;

        rigidbodyPlayer.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnDestroy()
    {
        rsoPlayerPosition.Value = Vector3.zero;
        rsoPlayerRotation.Value = Quaternion.identity;

    }

    private void OnEnable()
    {
        rsoPlayerPosition.Value = transform.position;
        rsoPlayerRotation.Value = transform.rotation;

        rseOnPlayerMove.action += Move;
        rseOnNewTargeting.action += ChangeNewTarget;
        rseOnPlayerCancelTargeting.action += CancelTarget;
    }

    private void OnDisable()
    {
        rseOnPlayerMove.action -= Move;
        rseOnNewTargeting.action -= ChangeNewTarget;
        rseOnPlayerCancelTargeting.action -= CancelTarget;
    }

    private void ChangeNewTarget(GameObject newTarget)
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

        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Moving) == false)
        {
            //inputCanceledOrNoInput = true;
            return;
        }

        if (moveInput != Vector2.zero)
        {
            rseOnAnimationBoolValueChange.Call(moveParam, true);
            inputCanceledOrNoInput = false;
        }
        else
        {
            rseOnAnimationBoolValueChange.Call(moveParam, false);
            inputCanceledOrNoInput = true;
        }

        if (moveInput.sqrMagnitude > 0.0001f)
        {
            _onPlayerAddState.Call(PlayerState.Moving);
        }
        else
        {
            _onPlayerAddState.Call(PlayerState.None);
        }
    }

    private void Update()
    {
        if (rsoPlayerIsTargeting.Value && target != null && _playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Moving) == true)
        {
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0f; // Ignore the heigth

            if (directionToTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                rigidbodyPlayer.MoveRotation(Quaternion.Slerp(rigidbodyPlayer.rotation, targetRotation, _playerStats.Value.turnSpeedTargeting * Time.fixedDeltaTime));
            }

            Vector3 right = Vector3.Cross(Vector3.up, directionToTarget.normalized);
            Vector3 forward = directionToTarget.normalized;

            Vector3 desiredDirection = right * moveInput.x + forward * moveInput.y;
            desiredDirection.Normalize();

            float inputMagnitude = Mathf.Clamp01(moveInput.magnitude);
            Vector3 desiredVelocity = desiredDirection * _playerStats.Value.strafeSpeed * inputMagnitude;


            
            Vector3 velocityTargeting = rigidbodyPlayer.linearVelocity;
            velocityTargeting.x = desiredVelocity.x;
            velocityTargeting.z = desiredVelocity.z;
            rigidbodyPlayer.linearVelocity = velocityTargeting;

            rsoPlayerPosition.Value = transform.position;
            rsoPlayerRotation.Value = transform.rotation;

            if (moveInput.sqrMagnitude > 0.0001f)
            {
                rseOnAnimationBoolValueChange.Call(moveParam, true);
                rseOnAnimationFloatValueChange.Call(speedParam, velocityTargeting.magnitude);
                rseOnAnimationFloatValueChange.Call(_strafXParam, moveInput.x);
                rseOnAnimationFloatValueChange.Call(_strafYParam, moveInput.y);
            }
            else
            {
                rseOnAnimationFloatValueChange.Call(speedParam, 0);
                rseOnAnimationFloatValueChange.Call(_strafXParam, 0f);
                rseOnAnimationFloatValueChange.Call(_strafYParam, 0f);
                rseOnAnimationBoolValueChange.Call(moveParam, false);
            }
           

            return;
        }
    }

    private void FixedUpdate()
    {
        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Moving) == false)
        {
            rseOnAnimationFloatValueChange.Call(speedParam, 0);
            rseOnAnimationFloatValueChange.Call(_strafXParam, 0f);
            rseOnAnimationFloatValueChange.Call(_strafYParam, 0f);
            rseOnAnimationBoolValueChange.Call(moveParam, false);
            //return;
        }
        

        if (rsoCurrentInputActionMap.Value == EnumPlayerInputActionMap.Game)
        {
            if (!rsoPlayerIsTargeting.Value && _playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Moving) == true)
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
                    rigidbodyPlayer.MoveRotation(Quaternion.Slerp(rigidbodyPlayer.rotation, target, _playerStats.Value.turnSpeed * Time.fixedDeltaTime));
                }
                else
                {
                    rigidbodyPlayer.angularVelocity = Vector3.zero;
                    desiredDir = Vector3.zero;
                }

                float inputMag = Mathf.Clamp01(moveInput.magnitude);
                Vector3 desiredVel = desiredDir * _playerStats.Value.moveSpeed * inputMag;


                Vector3 velocity = rigidbodyPlayer.linearVelocity;
                velocity.x = desiredVel.x;
                velocity.z = desiredVel.z;

                
                rigidbodyPlayer.linearVelocity = velocity;

                rseOnAnimationFloatValueChange.Call(speedParam, velocity.magnitude);

                rsoPlayerPosition.Value = transform.position;
                rsoPlayerRotation.Value = transform.rotation;

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

