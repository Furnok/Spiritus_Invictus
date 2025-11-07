using System.Collections;
using UnityEngine;

public class S_PlayerDodge : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, S_AnimationName] string _dodgeParam;
    [SerializeField, S_AnimationName] string _dodgeDirXParam;
    [SerializeField, S_AnimationName] string _dodgeDirYParam;

    [Header("World / collision")]
    [SerializeField] CapsuleCollider _capsule;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask obstacleMask;
    float maxSlopeAngle => _playerStats.Value.maxSlopeAngle;
    float maxDownStepAngle => _playerStats.Value.maxSlopeAngle;
    [SerializeField] float edgeProbeDistance = 0.6f;
    [SerializeField] float edgeProbeHeight = 0.5f;
    [SerializeField] float stopFromWall = 0.03f;

    [Header("References")]
    [SerializeField] private SSO_PlayerStateTransitions _ssoPlayerStateTransitions;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] RSO_PlayerIsDodging _playerIsDodging;
    [SerializeField] RSO_PlayerIsTargeting _playerIsTargeting;
    [SerializeField] SSO_PlayerStateTransitions _playerStateTransitions;
    [SerializeField] RSO_PlayerCurrentState _playerCurrentState;
    [SerializeField] SSO_PlayerStats _playerStats;
    [SerializeField] RSO_AttackDataInDodgeableArea _attackDataInDodgeableArea;
    [SerializeField] RSO_AttackCanHitPlayer _attackCanHitPlayer;
    [SerializeField] SSO_PlayerConvictionData _playerConvictionData;
    [SerializeField] SSO_AnimationTransitionDelays _animationTransitionDelays;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerDodgeInput rseOnPlayerDodge;
    [SerializeField] private RSE_OnPlayerMove _rseOnPlayerMove;
    [SerializeField] private RSE_OnNewTargeting _rseOnNewTargeting;
    [SerializeField] private RSE_OnPlayerCancelTargeting _rseOnPlayerCancelTargeting;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;
    [SerializeField] private RSE_OnPlayerDodgeInputCancel _onPlayerDodgeInputCancel;

    [Header("Output")]
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;
    [SerializeField] private RSE_OnAnimationFloatValueChange rseOnAnimationFloatValueChange;
    [SerializeField] RSE_OnPlayerGainConviction _onPlayerGainConviction;

    Vector2 _moveInput;
    Transform _target = null;
    Coroutine _dodgeCoroutine;
    Coroutine _prepareRunCoroutine;
    float _linearDamping;
    bool _canRunAfterDodge = false;

    private void OnEnable()
    {
        rseOnPlayerDodge.action += TryDodge;
        _rseOnPlayerMove.action += Move;
        _rseOnNewTargeting.action += ChangeNewTarget;
        _rseOnPlayerCancelTargeting.action += CancelTarget;
        _rseOnPlayerGettingHit.action += CancelDodge;
        _onPlayerDodgeInputCancel.action += CancelInputdodge;

        _playerIsDodging.Value = false;
        _canRunAfterDodge = false;

    }

    private void OnDisable()
    {
        rseOnPlayerDodge.action -= TryDodge;
        _rseOnPlayerMove.action -= Move;
        _rseOnNewTargeting.action -= ChangeNewTarget;
        _rseOnPlayerCancelTargeting.action -= CancelTarget;
        _rseOnPlayerGettingHit.action -= CancelDodge;
        _onPlayerDodgeInputCancel.action -= CancelInputdodge;

    }

    private void Awake()
    {
        _playerIsDodging.Value = false;
        _linearDamping = _rb.linearDamping;
    }

    private void ChangeNewTarget(GameObject newTarget)
    {
        _target = newTarget.transform;
    }

    private void CancelTarget(GameObject oldTarget)
    {
        _target = null;
    }
    private void TryDodge()
    {
        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Dodging) == false) return;
        _onPlayerAddState.Call(PlayerState.Dodging);

        //Test triggerDodgePerfect
        var isDodgePrefect = _attackDataInDodgeableArea.Value.Count > 0;
        if (isDodgePrefect)
        { 
            Debug.Log("Dodge perfect");
            _onPlayerGainConviction.Call(_playerConvictionData.Value.dodgeSuccessGain);

            foreach (var attackData in _attackDataInDodgeableArea.Value)
            {
                _attackCanHitPlayer.Value.Remove(attackData.Key);
            }
        }

        Vector3 dodgeDirection = Vector3.zero;

        if (_playerIsTargeting.Value == false)
        {
            dodgeDirection = transform.forward;

        }
        else
        {
            dodgeDirection = transform.forward * _moveInput.y + transform.right * _moveInput.x;

        }

        if (dodgeDirection == Vector3.zero)
        {

            dodgeDirection = -transform.forward;
        }

        dodgeDirection.Normalize();

        rseOnAnimationFloatValueChange.Call(_dodgeDirXParam, dodgeDirection.x);
        rseOnAnimationFloatValueChange.Call(_dodgeDirYParam, dodgeDirection.z);

        rseOnAnimationBoolValueChange.Call(_dodgeParam, true);

        _playerIsDodging.Value = true;

        if (_dodgeCoroutine != null) StopCoroutine(_dodgeCoroutine);
        _dodgeCoroutine = StartCoroutine(StartupAndDash(dodgeDirection));
    }

   

    
    IEnumerator StartupAndDash(Vector3 dodgeDir)
    {
        float startup = _animationTransitionDelays.Value.dodgeStartupDelay;
        if (startup > 0f)
        {
            yield return new WaitForSeconds(startup);
        }

        _rb.linearDamping = 0f;
        _rb.angularVelocity = Vector3.zero;
        _playerIsDodging.Value = true;
        _canRunAfterDodge = true;

        float dur = _playerStats.Value.dodgeDuration;
        float wantedDist = _playerStats.Value.dodgeDistance;
        AnimationCurve curve = _playerStats.Value._speedDodgeCurve;

        Vector3 groundNormal;
        bool onGround = CheckGround(out groundNormal);
        if (onGround)
        {
            dodgeDir = Vector3.ProjectOnPlane(dodgeDir, groundNormal).normalized;
        }
        else
        {
            dodgeDir = dodgeDir.normalized;
        }

        float elapsed = 0f;
        float travelled = 0f;

        Vector3 startPos = transform.position;

        while (elapsed < dur)
        {
            float t = elapsed / dur;
            float speedMul = curve != null ? curve.Evaluate(t) : 1f;

            float frameDist = (wantedDist / dur) * speedMul * Time.deltaTime;

            float remaining = wantedDist - travelled;
            if (remaining <= 0f) break;
            if (frameDist > remaining) frameDist = remaining;

            onGround = CheckGround(out groundNormal);
            Vector3 stepDir = dodgeDir;
            if (onGround)
            {
                stepDir = Vector3.ProjectOnPlane(stepDir, groundNormal).normalized;
            }

            float allowed = ProbeObstacle(stepDir, frameDist);
            if (allowed <= 0f)
            {
                break;
            }

            allowed = ProbeGroundAhead(stepDir, allowed, groundNormal);
            if (allowed <= 0f)
            {
                break;
            }

            Vector3 nextPos = transform.position + stepDir * allowed;
            _rb.MovePosition(nextPos);

            travelled += allowed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        _rb.linearVelocity = Vector3.zero;
        _rb.linearDamping = _linearDamping;
        _playerIsDodging.Value = false;
        rseOnAnimationBoolValueChange.Call(_dodgeParam, false);

        float rec = _animationTransitionDelays.Value.dodgeRecoveryDelay;
        if (rec > 0f)
        {
            yield return new WaitForSeconds(rec);
        }

        _onPlayerAddState.Call(PlayerState.None);

        yield return new WaitForSeconds(_playerStats.Value.delayBeforeRunningAfterDodge);
        if (_canRunAfterDodge && _playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Running))
        {
            _onPlayerAddState.Call(PlayerState.Running);
        }

        _dodgeCoroutine = null;
    }

    private void Move(Vector2 input)
    {
        _moveInput = input;
    }

    void CancelDodge()
    {
        _canRunAfterDodge = false;
        _playerIsDodging.Value = false;

        if (_dodgeCoroutine != null) StopCoroutine(_dodgeCoroutine);
        _rb.linearDamping = _linearDamping;
        rseOnAnimationBoolValueChange.Call(_dodgeParam, false);

        if (_prepareRunCoroutine != null) StopCoroutine(_prepareRunCoroutine);


        ResetValue();
    }

    void CancelInputdodge()
    {
        _canRunAfterDodge = false;

        //if (_playerCurrentState.Value != PlayerState.Running /*&& _prepareRunCoroutine != null*/ && _dodgeCoroutine == null)
        //{
        //    //StopCoroutine(_prepareRunCoroutine);
        //    _onPlayerAddState.Call(PlayerState.None);
        //}
        
        if(_playerCurrentState.Value == PlayerState.Running /*&& _dodgeCoroutine != null*/)
        {
            _onPlayerAddState.Call(PlayerState.None);
        }
    }

    private void ResetValue()
    {
        _playerIsDodging.Value = false;
        _canRunAfterDodge = false;
        rseOnAnimationBoolValueChange.Call(_dodgeParam, false);
    }

    void GetCapsuleWorldEnds(out Vector3 top, out Vector3 bottom, out float radius)
    {
        float height = Mathf.Max(_capsule.height * Mathf.Abs(transform.lossyScale.y), _capsule.radius * 2f);
        radius = _capsule.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x), Mathf.Abs(transform.lossyScale.z));
        Vector3 center = transform.TransformPoint(_capsule.center);
        Vector3 up = transform.up;
        top = center + up * (height * 0.5f - radius);
        bottom = center - up * (height * 0.5f - radius);
    }

    bool CheckGround(out Vector3 normal)
    {
        GetCapsuleWorldEnds(out var top, out var bottom, out var radius);
        Vector3 start = bottom + Vector3.up * 0.02f;
        float dist = 0.6f;

        if (Physics.Raycast(start, Vector3.down, out var hit, dist, groundMask, QueryTriggerInteraction.Ignore))
        {
            float ang = Vector3.Angle(hit.normal, Vector3.up);
            if (ang <= maxSlopeAngle)
            {
                normal = hit.normal;
                return true;
            }
        }
        normal = Vector3.up;
        return false;
    }

    float ProbeObstacle(Vector3 dir, float maxDist)
    {
        GetCapsuleWorldEnds(out var top, out var bottom, out var radius);

        if (Physics.CapsuleCast(bottom, top, radius, dir, out var hit, maxDist, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            return Mathf.Max(0f, hit.distance - stopFromWall);
        }
        return maxDist;
    }

    float ProbeGroundAhead(Vector3 dir, float maxDist, Vector3 currentGroundNormal)
    {
        Vector3 probePos = transform.position + dir * maxDist + Vector3.up * edgeProbeHeight;
        if (Physics.Raycast(probePos, Vector3.down, out var hit, edgeProbeHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
        {
            float ang = Vector3.Angle(hit.normal, Vector3.up);
            if (ang > maxDownStepAngle) return 0f;
            return maxDist;
        }
        return 0f;
    }
}