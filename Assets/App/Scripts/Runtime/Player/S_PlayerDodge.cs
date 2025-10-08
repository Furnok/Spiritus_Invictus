using UnityEngine;

public class S_PlayerDodge : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _dodgeForce = 12f;
    [SerializeField] private float _dodgeDuration = 0.5f;
    [SerializeField] private AnimationCurve _speedCurve;

    [Header("References")]
    [SerializeField] private SSO_PlayerStateTransitions _ssoPlayerStateTransitions;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] RSO_PlayerIsDodging _playerIsDodging;
    [SerializeField] RSO_PlayerIsTargeting _playerIsTargeting;


    [Header("Input")]
    [SerializeField] private RSE_OnPlayerDodgeInput rseOnPlayerDodge;
    [SerializeField] private RSE_OnPlayerMove _rseOnPlayerMove;
    [SerializeField] private RSE_OnNewTargeting _rseOnNewTargeting;
    [SerializeField] private RSE_OnPlayerCancelTargeting _rseOnPlayerCancelTargeting;

    Vector2 _moveInput;
    Transform _target = null;

    private void OnEnable()
    {
        rseOnPlayerDodge.action += TryDodge;
        _rseOnPlayerMove.action += Move;
        _rseOnNewTargeting.action += ChangeNewTarget;
        _rseOnPlayerCancelTargeting.action += CancelTarget;

        _playerIsDodging.Value = false;
    }

    private void OnDisable()
    {
        rseOnPlayerDodge.action -= TryDodge;
        _rseOnPlayerMove.action -= Move;
        _rseOnNewTargeting.action -= ChangeNewTarget;
        _rseOnPlayerCancelTargeting.action -= CancelTarget;
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
        // Check if can dodge
        if (_playerIsDodging.Value) return;

        if (true)
        {
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

            _playerIsDodging.Value = true;

            StartCoroutine(PerformDodge(dodgeDirection));

        }
    }

    System.Collections.IEnumerator PerformDodge(Vector3 dodgeDirection)
    {
        var elapsed = 0f;

        _rb.linearDamping = 0;
        _rb.angularVelocity = Vector3.zero;

        while (elapsed < _dodgeDuration)
        {
            float t = elapsed / _dodgeDuration;
            float speed = _speedCurve != null ? _speedCurve.Evaluate(t) : 1f;

            _rb.linearVelocity = dodgeDirection * _dodgeForce * speed;

            elapsed += Time.deltaTime;
            yield return null;
        }

        _rb.linearVelocity = Vector3.zero;
        _rb.linearDamping = 5;

        _playerIsDodging.Value = false;

        //_rseOnDodgeEnd.RaiseEvent();
    }

    private void Move(Vector2 input)
    {
        if(_playerIsDodging.Value) return;

        _moveInput = input;
    }
}