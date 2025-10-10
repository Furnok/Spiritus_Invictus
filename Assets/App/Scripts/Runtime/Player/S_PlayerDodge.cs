using UnityEngine;

public class S_PlayerDodge : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, S_AnimationName] string _dodgeParam;
    [SerializeField, S_AnimationName] string _dodgeDirXParam;
    [SerializeField, S_AnimationName] string _dodgeDirYParam;


    [Header("References")]
    [SerializeField] private SSO_PlayerStateTransitions _ssoPlayerStateTransitions;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] RSO_PlayerIsDodging _playerIsDodging;
    [SerializeField] RSO_PlayerIsTargeting _playerIsTargeting;
    [SerializeField] SSO_PlayerStateTransitions _playerStateTransitions;
    [SerializeField] RSO_PlayerCurrentState _playerCurrentState;
    [SerializeField] SSO_PlayerStats _playerStats;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerDodgeInput rseOnPlayerDodge;
    [SerializeField] private RSE_OnPlayerMove _rseOnPlayerMove;
    [SerializeField] private RSE_OnNewTargeting _rseOnNewTargeting;
    [SerializeField] private RSE_OnPlayerCancelTargeting _rseOnPlayerCancelTargeting;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;

    [Header("Output")]
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;
    [SerializeField] private RSE_OnAnimationFloatValueChange rseOnAnimationFloatValueChange;

    Vector2 _moveInput;
    Transform _target = null;
    Coroutine _dodgeCoroutine;

    private void OnEnable()
    {
        rseOnPlayerDodge.action += TryDodge;
        _rseOnPlayerMove.action += Move;
        _rseOnNewTargeting.action += ChangeNewTarget;
        _rseOnPlayerCancelTargeting.action += CancelTarget;
        _rseOnPlayerGettingHit.action += CancelDodge;

        _playerIsDodging.Value = false;
    }

    private void OnDisable()
    {
        rseOnPlayerDodge.action -= TryDodge;
        _rseOnPlayerMove.action -= Move;
        _rseOnNewTargeting.action -= ChangeNewTarget;
        _rseOnPlayerCancelTargeting.action -= CancelTarget;
        _rseOnPlayerGettingHit.action -= CancelDodge;

    }

    private void Awake()
    {
        _playerIsDodging.Value = false;
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

        _dodgeCoroutine = StartCoroutine(PerformDodge(dodgeDirection));
    }

    System.Collections.IEnumerator PerformDodge(Vector3 dodgeDirection)
    {
        var elapsed = 0f;

        _rb.linearDamping = 0;
        _rb.angularVelocity = Vector3.zero;

        while (elapsed < _playerStats.Value.dodgeDuration)
        {
            float t = elapsed / _playerStats.Value.dodgeDuration;
            float speed = _playerStats.Value._speedDodgeCurve != null ? _playerStats.Value._speedDodgeCurve.Evaluate(t) : 1f;

            _rb.linearVelocity = dodgeDirection * _playerStats.Value.dodgeForce * speed;

            elapsed += Time.deltaTime;
            yield return null;
        }

        _rb.linearVelocity = Vector3.zero;
        _rb.linearDamping = 5;

        ResetValue();
    }

    private void Move(Vector2 input)
    {
        _moveInput = input;
    }

    void CancelDodge()
    {
        if (_dodgeCoroutine != null) return;
        StopCoroutine(_dodgeCoroutine);
        ResetValue();
    }

    private void ResetValue()
    {
        _playerIsDodging.Value = false;
        _onPlayerAddState.Call(PlayerState.None);
        rseOnAnimationBoolValueChange.Call(_dodgeParam, false);
    }
}