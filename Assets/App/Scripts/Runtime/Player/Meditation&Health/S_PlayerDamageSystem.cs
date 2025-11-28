using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerDamageSystem : MonoBehaviour
{
    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerHit _rseOnPlayerHit;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerHealthReduced rseOnPlayerHealthReduced;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnAnimationTriggerValueChange rseOnAnimationTriggerValueChange;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerAddState _onPlayerAddState;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerCurrentState _playerCurrentState;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_IsInvicible _isInvicible;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_ConsoleCheats _debugPlayer;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerStateTransitions _playerStateTransitions;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerStats _playerStats;

    private Coroutine _hitReactCoroutine = null;

    private void OnEnable()
    {
        _isInvicible.Value = false;

        _rseOnPlayerHit.action += TakeDamage;
    }
    
    private void OnDisable()
    {
        _rseOnPlayerHit.action -= TakeDamage;
    }

    private void TakeDamage(S_StructAttackContact attackContact)
    {
        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.HitReact) == true && _isInvicible.Value == false && _debugPlayer.Value.cantGetttingHit == false)
        {
            var attackData = attackContact.data;

            if (_hitReactCoroutine != null)
            {
                StopCoroutine(_hitReactCoroutine);
            }

            rseOnAnimationTriggerValueChange.Call("isHit");
            _onPlayerAddState.Call(PlayerState.HitReact);
            _isInvicible.Value = true;

            _hitReactCoroutine = StartCoroutine(S_Utils.Delay(attackData.knockbackHitDuration, () =>
            {
                if(_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.None) == true)
                {
                    _onPlayerAddState.Call(PlayerState.None);
                }
            }));

            StartCoroutine(S_Utils.Delay(attackData.invicibilityDuration, () =>
            {
                _isInvicible.Value = false;
            }));

            rseOnPlayerHealthReduced.Call(attackData.damage);
            _rseOnPlayerGettingHit.Call();
        }
    }
}