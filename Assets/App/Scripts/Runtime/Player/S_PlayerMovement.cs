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
    [SerializeField] private RSE_OnPlayerMoveInputCancel _onPlayerMoveInputCancel;

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
    bool _isInputCanceled = false;

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
        _onPlayerMoveInputCancel.action += OnCancelInput;
    }

    private void OnDisable()
    {
        rseOnPlayerMove.action -= Move;
        rseOnNewTargeting.action -= ChangeNewTarget;
        rseOnPlayerCancelTargeting.action -= CancelTarget;
        _onPlayerMoveInputCancel.action -= OnCancelInput;
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

        if (moveInput != Vector2.zero)
        {
            inputCanceledOrNoInput = false;
        }
        else
        {
            inputCanceledOrNoInput = true;
        }
    }

    void OnCancelInput()
    {
        _isInputCanceled = true;
    }

    private void Update()
    {
        if (rsoPlayerIsTargeting.Value && target != null && _playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Moving) == true || rsoPlayerIsTargeting.Value && target != null && _playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Running) == true)
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

            float inputMag = Mathf.Clamp01(moveInput.magnitude);
            var multiplierSpeed = _playerCurrentState.Value == PlayerState.Running ? _playerStats.Value.runSpeed : _playerStats.Value.strafeSpeed;
            Vector3 desiredHorizontalVel = desiredDirection * multiplierSpeed * inputMag;

            // Get current velocity
            Vector3 vel = rigidbodyPlayer.linearVelocity;

            // Replace only XZ components
            vel.x = desiredHorizontalVel.x;
            vel.z = desiredHorizontalVel.z;

            // Reassign velocity once
            rigidbodyPlayer.linearVelocity = vel;

            rsoPlayerPosition.Value = transform.position;
            rsoPlayerRotation.Value = transform.rotation;

            if (moveInput.sqrMagnitude > 0.0001f)
            {
                rseOnAnimationBoolValueChange.Call(moveParam, true);
                rseOnAnimationFloatValueChange.Call(speedParam, vel.magnitude);
                
                if (_playerCurrentState.Value != PlayerState.Running)
                {
                    _onPlayerAddState.Call(PlayerState.Moving);
                    rseOnAnimationFloatValueChange.Call(_strafXParam, moveInput.x);
                    rseOnAnimationFloatValueChange.Call(_strafYParam, moveInput.y);
                }

            }
            else
            {
                rseOnAnimationFloatValueChange.Call(speedParam, 0);
                rseOnAnimationFloatValueChange.Call(_strafXParam, 0f);
                rseOnAnimationFloatValueChange.Call(_strafYParam, 0f);
                rseOnAnimationBoolValueChange.Call(moveParam, false);
                _onPlayerAddState.Call(PlayerState.None);

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

            if (_playerCurrentState.Value != PlayerState.Dodging) // Allow movement after dodging
            { 
                rigidbodyPlayer.linearVelocity = Vector3.zero;
            }

            //return;
        }



        if (rsoCurrentInputActionMap.Value == EnumPlayerInputActionMap.Game)
        {
            if (!rsoPlayerIsTargeting.Value && _playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Moving) == true || !rsoPlayerIsTargeting.Value && _playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Running) == true)
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

                var multiplierSpeed = _playerCurrentState.Value == PlayerState.Running ? _playerStats.Value.runSpeed : _playerStats.Value.moveSpeed;
                Vector3 desiredHorizontalVel = desiredDir * multiplierSpeed * inputMag;

                // Get current velocity
                Vector3 vel = rigidbodyPlayer.linearVelocity;

                // Replace only XZ components
                vel.x = desiredHorizontalVel.x;
                vel.z = desiredHorizontalVel.z;

                // Reassign velocity once
                rigidbodyPlayer.linearVelocity = vel;

                if (moveInput.sqrMagnitude > 0.0001f && _playerCurrentState.Value != PlayerState.Running)
                {
                    rseOnAnimationFloatValueChange.Call(speedParam, vel.magnitude);
                    rseOnAnimationBoolValueChange.Call(moveParam, true);
                    _onPlayerAddState.Call(PlayerState.Moving);

                    //if (_playerCurrentState.Value != PlayerState.Running)
                    //{
                    //}

                }
                else if (_isInputCanceled == false && _playerCurrentState.Value == PlayerState.Running)
                {
                    rseOnAnimationFloatValueChange.Call(speedParam, vel.magnitude);
                    rseOnAnimationBoolValueChange.Call(moveParam, true);
                }
                else
                {
                    rseOnAnimationFloatValueChange.Call(speedParam, 0);
                    rseOnAnimationBoolValueChange.Call(moveParam, false);
                    _onPlayerAddState.Call(PlayerState.None);
                    _isInputCanceled = false;
                }

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

