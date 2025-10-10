using UnityEngine;

public class S_PlayerHeal : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, S_AnimationName] string _healParam;

    [Header("References")]
    [SerializeField] SSO_PlayerStateTransitions _playerStateTransitions;
    [SerializeField] RSO_PlayerCurrentState _playerCurrentState;
    [SerializeField] private SSO_PlayerStats ssoPlayerStats;
    [SerializeField] RSO_PlayerCurrentHealth _playerCurrentHealth;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerHealInput rseOnPlayerHeal;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;

    [Header("Output")]
    [SerializeField] private RSE_OnPlayerHealPerformed rseOnPlayerHealPerformed;
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;

    private Coroutine healCoroutine = null;

    private void OnEnable()
    {
        rseOnPlayerHeal.action += TryHeal;
        _rseOnPlayerGettingHit.action += CancelHeal;
    }

    private void OnDisable()
    {
        rseOnPlayerHeal.action -= TryHeal;
        _rseOnPlayerGettingHit.action -= CancelHeal;
    }

    private void TryHeal()
    {
        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Healing) == false) return;
        if (_playerCurrentHealth.Value >= ssoPlayerStats.Value.maxHealth) return;

        _onPlayerAddState.Call(PlayerState.Healing);
        rseOnAnimationBoolValueChange.Call(_healParam, true); 

        healCoroutine = StartCoroutine(S_Utils.Delay(ssoPlayerStats.Value.delayBeforeHeal, () =>
        {
            rseOnPlayerHealPerformed.Call();
            _onPlayerAddState.Call(PlayerState.None);
            rseOnAnimationBoolValueChange.Call(_healParam, false);
        }));
    }

    private void CancelHeal()
    {
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
            _onPlayerAddState.Call(PlayerState.None);
            rseOnAnimationBoolValueChange.Call(_healParam, false);
        }
    }
}