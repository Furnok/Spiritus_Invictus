using DG.Tweening.Core.Easing;
using UnityEngine;

public class S_PlayerParry : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, S_AnimationName] string _parryParam;


    [Header("References")]
    [SerializeField] RSO_CanParry _canParry;
    [SerializeField] RSO_ParryStartTime _parryStartTime;
    [SerializeField] SSO_PlayerStats _playerStats;
    [SerializeField] SSO_PlayerStateTransitions _playerStateTransitions;
    [SerializeField] RSO_PlayerCurrentState _playerCurrentState;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerParryInput rseOnPlayerParry;

    [Header("Output")]
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;

    float _parryDuration => _playerStats.Value.parryDuration;



    private void Awake()
    {
        _canParry.Value = false;
        _parryStartTime.Value = 0f;
    }
    private void OnEnable()
    {
        rseOnPlayerParry.action += TryParry;
    }

    private void OnDisable()
    {
        rseOnPlayerParry.action -= TryParry;
    }

    private void TryParry()
    {
        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Parrying) == false) return;
        _onPlayerAddState.Call(PlayerState.Parrying);
        
        _parryStartTime.Value = Time.time;
        _canParry.Value = true;
        rseOnAnimationBoolValueChange.Call(_parryParam, true);

        StartCoroutine(S_Utils.Delay(_parryDuration, () =>
        {
            _onPlayerAddState.Call(PlayerState.None);

            _canParry.Value = false;
            rseOnAnimationBoolValueChange.Call(_parryParam, false);
        }));
    }
}