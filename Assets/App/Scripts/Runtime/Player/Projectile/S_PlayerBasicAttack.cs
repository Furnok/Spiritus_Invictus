using UnityEngine;

public class S_PlayerBasicAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 attackOffset;
    [SerializeField] SSO_PlayerStateTransitions _playerStateTransitions;
    [SerializeField] RSO_PlayerCurrentState _playerCurrentState;
    [SerializeField, S_AnimationName] string _attackParam;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerAttackInput rseOnPlayerAttack;

    [Header("Output")]
    [SerializeField] private SSO_BasicAttackDelayIncantation ssoDelayIncantationAttack;
    [SerializeField] private RSE_OnSpawnProjectile rseOnSpawnProjectile;
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;

    private void OnEnable()
    {
        rseOnPlayerAttack.action += OnPlayerAttackInput;
    }

    private void OnDisable()
    {
        rseOnPlayerAttack.action -= OnPlayerAttackInput;
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

        StartCoroutine(S_Utils.Delay(ssoDelayIncantationAttack.Value, () =>
        {
            rseOnSpawnProjectile.Call(attackposition);

            rseOnAnimationBoolValueChange.Call(_attackParam, false);

            _onPlayerAddState.Call(PlayerState.None);
        }));
    }
}

