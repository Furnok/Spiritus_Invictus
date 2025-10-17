using System.Linq;
using UnityEngine;

public class S_PlayerBasicAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 attackOffset;
    [SerializeField] SSO_PlayerStateTransitions _playerStateTransitions;
    [SerializeField] RSO_PlayerCurrentState _playerCurrentState;
    [SerializeField, S_AnimationName] string _attackParam;

    [Header("Reference")]
    [SerializeField] SSO_PlayerConvictionData _playerConvictionData;
    [SerializeField] SSO_PlayerStats _playerStats;
    [SerializeField] RSO_PlayerCurrentConviction _playerCurrentConviction;
    [SerializeField] SSO_PlayerAttackSteps _playerAttackSteps;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerAttackInput rseOnPlayerAttack;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;
   
    [SerializeField] RSE_OnPlayerAttackInputCancel _onPlayerAttackInputCancel;

    [Header("Output")]
    [SerializeField] private SSO_BasicAttackDelayIncantation ssoDelayIncantationAttack;
    [SerializeField] private RSE_OnSpawnProjectile rseOnSpawnProjectile;
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;
    [SerializeField] RSE_OnPlayerAttackCancel _onPlayerAttackCancel;

    Coroutine _attackCoroutine;

    private void Awake()
    {
        var steps = _playerAttackSteps.Value;
        if (steps == null || steps.Count == 0)
        {
            Debug.LogWarning("No steps configured in the SSO_PlayerAttackSteps");
            return;
        }

        var duplicateSteps = steps
            .GroupBy(s => s.step)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateSteps.Count > 0)
        {
            Debug.LogError("Duplicate steps detected: SSO_PlayerAttackSteps");
        }
    }

    private void OnEnable()
    {
        rseOnPlayerAttack.action += OnPlayerAttackInput;
        _rseOnPlayerGettingHit.action += CancelAttack;
        _onPlayerAttackInputCancel.action += OnPlayerAttackInputCancel;
    }

    private void OnDisable()
    {
        rseOnPlayerAttack.action -= OnPlayerAttackInput;
        _rseOnPlayerGettingHit.action -= CancelAttack;
        _onPlayerAttackInputCancel.action -= OnPlayerAttackInputCancel;
    }

    private void OnPlayerAttackInput()
    {
        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Attacking) == false) return;
        _onPlayerAddState.Call(PlayerState.Attacking);

        S_StructProjectileData attackposition = new S_StructProjectileData
        {
            locationSpawn = transform.position + transform.TransformVector(attackOffset),
            direction = transform.forward
        };

        rseOnAnimationBoolValueChange.Call(_attackParam, true);

        _attackCoroutine = StartCoroutine(S_Utils.Delay(ssoDelayIncantationAttack.Value, () =>
        {
            rseOnSpawnProjectile.Call(attackposition);

            rseOnAnimationBoolValueChange.Call(_attackParam, false);

            _onPlayerAddState.Call(PlayerState.None);
        }));
    }

    void OnPlayerAttackInputCancel()
    {

    }

    void CancelAttack()
    {
        if ( _attackCoroutine == null ) return;
        StopCoroutine(_attackCoroutine);
        rseOnAnimationBoolValueChange.Call(_attackParam, false);

        _onPlayerAttackCancel.Call(0); //Add the step of the attack

        _onPlayerAddState.Call(PlayerState.None);
    }
}

