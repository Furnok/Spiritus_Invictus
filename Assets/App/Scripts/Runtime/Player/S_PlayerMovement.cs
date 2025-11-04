using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class S_PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField][S_AnimationName] private string moveParam;
    [SerializeField][S_AnimationName] private string speedParam;
    [SerializeField, S_AnimationName] string _strafXParam;
    [SerializeField, S_AnimationName] string _strafYParam;

    [Header("Grounding")]
    [SerializeField] private CapsuleCollider capsule;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDist = 0.5f;
    [SerializeField] private float maxSlopeAngle = 45f;
    [SerializeField] private float skin = 0.02f;

    [Header("Stick To Ground")]
    [SerializeField] private float stickToGroundForce = 12f;

    [Header("Edge Guard")]
    [SerializeField] private bool preventFallFromEdges = true;
    [SerializeField] private float edgeProbeDistance = 0.6f;
    [SerializeField] private float edgeProbeHeight = 0.5f;
    [SerializeField] private float maxDownStepAngle = 40f;

    [Header("Slope Slowdown")]
    [SerializeField, Range(0f, 60f)] private float slopeSlowStart = 5f;
    [SerializeField, Range(0f, 60f)] private float slopeSlowMax = 45f;
    [SerializeField, Range(0f, 0.9f)] private float slopeSlowAtMax = 0.10f;

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

    [SerializeField] RSE_OnParrySuccess _rseOnParrySuccess;
    [SerializeField] RSE_OnPlayerHit _rseOnPlayerHit;

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

    bool isGrounded;
    Vector3 groundNormal = Vector3.up;
    float groundAngle;

    Coroutine knockbackCoroutine;

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

        _rseOnParrySuccess.action += DoKnockback;
        _rseOnPlayerHit.action += DoKnockback;
    }

    private void OnDisable()
    {
        rseOnPlayerMove.action -= Move;
        rseOnNewTargeting.action -= ChangeNewTarget;
        rseOnPlayerCancelTargeting.action -= CancelTarget;
        _onPlayerMoveInputCancel.action -= OnCancelInput;

        _rseOnParrySuccess.action -= DoKnockback;
        _rseOnPlayerHit.action -= DoKnockback;
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
        
    }

    private void FixedUpdate()
    {
        UpdateGround();

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
        }



        if (rsoCurrentInputActionMap.Value == EnumPlayerInputActionMap.Game)
        {
            if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Moving) == true ||
                _playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Running) == true)
            {
                BuildDesiredDirection(out Vector3 desiredDir, out float baseSpeed);

                if (rsoPlayerIsTargeting.Value && target != null)
                {
                    Vector3 toTarget = target.position - transform.position;
                    toTarget.y = 0f;
                    if (toTarget.sqrMagnitude > 1e-4f)
                    {
                        Quaternion face = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                        rigidbodyPlayer.MoveRotation(
                            Quaternion.Slerp(rigidbodyPlayer.rotation, face, _playerStats.Value.turnSpeedTargeting * Time.fixedDeltaTime)
                        );
                    }
                }
                else if (desiredDir.sqrMagnitude > 1e-6f)
                {
                    Quaternion face = Quaternion.LookRotation(desiredDir, Vector3.up);
                    rigidbodyPlayer.MoveRotation(
                        Quaternion.Slerp(rigidbodyPlayer.rotation, face, _playerStats.Value.turnSpeed * Time.fixedDeltaTime)
                    );
                }
                else
                {
                    rigidbodyPlayer.angularVelocity = Vector3.zero;
                }

                Vector3 groundDir = desiredDir;
                if (isGrounded && groundDir.sqrMagnitude > 1e-6f)
                {
                    groundDir = Vector3.ProjectOnPlane(groundDir, groundNormal).normalized;
                }

                float speedMul = 1f;
                if (isGrounded && groundDir.sqrMagnitude > 1e-6f)
                {
                    Vector3 upslope = Vector3.ProjectOnPlane(Vector3.up, groundNormal);
                    float upslopeMag = upslope.magnitude;

                    if (upslopeMag > 1e-6f)
                    {
                        upslope /= upslopeMag;
                        float uphillDot = Vector3.Dot(groundDir, upslope);
                        if (uphillDot > 0f)
                        {
                            float t = Mathf.InverseLerp(slopeSlowStart, slopeSlowMax, groundAngle);
                            float penalty = Mathf.Clamp01(t) * slopeSlowAtMax;
                            speedMul = 1f - penalty;

                            speedMul = 1f - (1f - speedMul) * uphillDot;
                            speedMul = Mathf.Clamp(speedMul, 0.1f, 1f);
                        }
                    }
                }
                baseSpeed *= speedMul;

                groundDir = LimitSteepDescend(groundDir);

                Vector3 desiredVel = groundDir * baseSpeed;
                if (preventFallFromEdges && desiredVel.sqrMagnitude > 1e-6f)
                {
                    Vector3 stepDir = desiredVel.normalized;
                    if (!HasGroundAhead(rigidbodyPlayer.position, stepDir * edgeProbeDistance, out _))
                        desiredVel = Vector3.zero;
                }

                Vector3 vel = rigidbodyPlayer.linearVelocity;
                vel.x = desiredVel.x;
                vel.z = desiredVel.z;

                if (isGrounded)
                {
                    float vn = Vector3.Dot(vel, groundNormal);
                    if (vn > 0f) vel -= groundNormal * vn;
                    const float targetVn = -0.6f;
                    if (vn > targetVn) vel += -groundNormal * (vn - targetVn);
                }
                rigidbodyPlayer.linearVelocity = vel;

                // Animations
                float horizSpeed = new Vector2(vel.x, vel.z).magnitude;
                rseOnAnimationFloatValueChange.Call(speedParam, horizSpeed);

                bool targetMode = rsoPlayerIsTargeting.Value && target != null;
                PushMovementAnims(targetMode, horizSpeed, moveInput);

                if (moveInput.sqrMagnitude > 0.0001f && _playerCurrentState.Value != PlayerState.Running)
                {
                    _onPlayerAddState.Call(PlayerState.Moving);
                }
                else if (!_isInputCanceled && _playerCurrentState.Value == PlayerState.Running)
                {
                }
                else
                {
                    _onPlayerAddState.Call(PlayerState.None);
                    _isInputCanceled = false;
                }

                rsoPlayerPosition.Value = transform.position;
                rsoPlayerRotation.Value = transform.rotation;
            }
        }
        else
        {
            rigidbodyPlayer.linearVelocity = Vector3.zero;
            rseOnAnimationFloatValueChange.Call(speedParam, 0);
        }
    }

    void DoKnockback(AttackContact attackContact)
    {
        //if (knockbackCoroutine != null)
        //{
        //    StopCoroutine(knockbackCoroutine);
        //}

        //knockbackCoroutine = StartCoroutine(KnockbackCoroutine(attackContact));
    }

    IEnumerator KnockbackCoroutine(AttackContact attackContact)
    {
        var attackData = attackContact.data;
        var knockbackDuration = attackData.knockbackDuration;
        var knockbackDistance = attackData.knockbackDistance;
        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            float step = knockbackDistance / knockbackDuration;

            transform.position += -transform.forward * step * Time.deltaTime;

            elapsed += Time.deltaTime;

            yield return null;
        }

        Vector3 fromDir = (transform.position - attackContact.source.transform.position).normalized;
        fromDir.y = 0f;
        fromDir.Normalize();

        yield return null;
    }

    void GetCapsuleWorldEnds(out Vector3 top, out Vector3 bottom, out float radius)
    {
        float height = Mathf.Max(capsule.height * Mathf.Abs(transform.lossyScale.y), capsule.radius * 2f);
        radius = capsule.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));
        Vector3 center = transform.TransformPoint(capsule.center);
        Vector3 up = transform.up;
        top = center + up * (height * 0.5f - radius);
        bottom = center - up * (height * 0.5f - radius);
    }

    bool CheckGround(out RaycastHit hit)
    {
        GetCapsuleWorldEnds(out var top, out var bottom, out var radius);

        Vector3 probeStart = bottom + Vector3.up * 0.02f;
        float dist = groundCheckDist + 0.05f;

        bool ok = Physics.Raycast(
            probeStart, Vector3.down, out hit, dist,
            groundMask, QueryTriggerInteraction.Ignore
        );

        if (ok)
        {
            float ang = Vector3.Angle(hit.normal, Vector3.up);
            if (ang > maxSlopeAngle) ok = false;
        }
        return ok;
    }

    void UpdateGround()
    {
        if (CheckGround(out var hit))
        {
            isGrounded = true;
            groundNormal = hit.normal;
            groundAngle = Vector3.Angle(groundNormal, Vector3.up);
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
            groundAngle = 90f;
        }
    }

    void ApplyGroundStick(ref Vector3 vel)
    {
        if (!isGrounded) return;

        if (vel.y > 0f) vel.y = 0f;
        rigidbodyPlayer.AddForce(Vector3.down * stickToGroundForce, ForceMode.Acceleration);

        if (CheckGround(out var hit) && rigidbodyPlayer.linearVelocity.magnitude <= 3f)
        {
            float corr = hit.distance - skin;
            if (Mathf.Abs(corr) < 0.03f)
            {
                rigidbodyPlayer.MovePosition(rigidbodyPlayer.position + Vector3.down * corr);
            }
        }
    }

    Vector3 LimitSteepDescend(Vector3 along)
    {
        if (!isGrounded) return along;
        if (groundAngle <= maxSlopeAngle) return along;

        Vector3 downhill = Vector3.Cross(Vector3.Cross(groundNormal, Vector3.up), groundNormal).normalized;
        float dotDown = Vector3.Dot(along, downhill);
        if (dotDown > 0f) along -= downhill * dotDown;
        return along.normalized;
    }

    bool HasGroundAhead(Vector3 currentPos, Vector3 horizontalStep, out RaycastHit hit)
    {
        Vector3 probeOrigin = currentPos + horizontalStep + Vector3.up * edgeProbeHeight;
        if (Physics.Raycast(probeOrigin, Vector3.down, out hit, edgeProbeHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
        {
            float ang = Vector3.Angle(hit.normal, Vector3.up);
            return ang <= maxDownStepAngle;
        }
        return false;
    }

    void BuildDesiredDirection(out Vector3 desiredDir, out float baseSpeed)
    {
        float inputMag = Mathf.Clamp01(moveInput.magnitude);
        bool running = (_playerCurrentState.Value == PlayerState.Running);

        if (rsoPlayerIsTargeting.Value && target != null)// If targeting mode
        {
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 1e-6f)
            {
                Vector3 forward = toTarget.normalized;
                Vector3 right = Vector3.Cross(Vector3.up, forward).normalized; //Strafe basis

                desiredDir = (right * moveInput.x + forward * moveInput.y);
            }
            else
            {
                desiredDir = Vector3.zero;
            }

            baseSpeed = (running ? _playerStats.Value.runSpeed : _playerStats.Value.strafeSpeed) * inputMag;
        }
        else
        {
            Quaternion camRot = (inputCanceledOrNoInput ? (rsoCameraRotation ? rsoCameraRotation.Value : Quaternion.identity) : camRotInInputPerformed);

            Vector3 camF = camRot * Vector3.forward; camF.y = 0f; camF.Normalize();
            Vector3 camR = camRot * Vector3.right; camR.y = 0f; camR.Normalize();

            desiredDir = camR * moveInput.x + camF * moveInput.y;
            baseSpeed = (running ? _playerStats.Value.runSpeed : _playerStats.Value.moveSpeed) * inputMag;

            if (inputCanceledOrNoInput) camRotInInputPerformed = camRot;
        }

        if (desiredDir.sqrMagnitude > 1e-6f) desiredDir.Normalize();
    }

    void PushMovementAnims(bool isTargetMode, float horizSpeed, Vector2 rawInput)
    {
        rseOnAnimationFloatValueChange.Call(speedParam, horizSpeed);

        if (rawInput.sqrMagnitude > 0.0001f)
        {
            rseOnAnimationBoolValueChange.Call(moveParam, true);

            if (isTargetMode)
            {
                rseOnAnimationFloatValueChange.Call(_strafXParam, rawInput.x);
                rseOnAnimationFloatValueChange.Call(_strafYParam, rawInput.y);
            }
            else
            {
                rseOnAnimationFloatValueChange.Call(_strafXParam, 0f);
                rseOnAnimationFloatValueChange.Call(_strafYParam, 0f);
            }
        }
        else
        {
            rseOnAnimationBoolValueChange.Call(moveParam, false);
            rseOnAnimationFloatValueChange.Call(_strafXParam, 0f);
            rseOnAnimationFloatValueChange.Call(_strafYParam, 0f);
            rseOnAnimationFloatValueChange.Call(speedParam, 0f);
        }
    }

}

