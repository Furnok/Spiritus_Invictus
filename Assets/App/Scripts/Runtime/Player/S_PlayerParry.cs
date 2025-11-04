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
    [SerializeField] SSO_AnimationTransitionDelays _animationTransitionDelays;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerParryInput rseOnPlayerParry;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;

    [Header("Output")]
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;

    float _parryDuration => _playerStats.Value.parryDuration;

    Coroutine _parryCoroutine;

    private void Awake()
    {
        _canParry.Value = false;
        _parryStartTime.Value = 0f;
    }
    private void OnEnable()
    {
        _canParry.Value = false;
        _parryStartTime.Value = 0f;

        rseOnPlayerParry.action += TryParry;
        _rseOnPlayerGettingHit.action += CancelParry;
    }

    private void OnDisable()
    {
        rseOnPlayerParry.action -= TryParry;
        _rseOnPlayerGettingHit.action -= CancelParry;
    }

    private void TryParry()
    {
        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Parrying) == false) return;
        _onPlayerAddState.Call(PlayerState.Parrying);

        if (_parryCoroutine != null)
        {
            StopCoroutine(_parryCoroutine);
        }

        rseOnAnimationBoolValueChange.Call(_parryParam, true);

        _parryCoroutine = StartCoroutine(S_Utils.Delay(_animationTransitionDelays.Value.parryStartupDelay, () =>
        {
            _parryStartTime.Value = Time.time;
            _canParry.Value = true;

            _parryCoroutine = StartCoroutine(S_Utils.Delay(_parryDuration, () =>
            {
                _canParry.Value = false;

                _parryCoroutine = StartCoroutine(S_Utils.Delay(_animationTransitionDelays.Value.parryRecoveryDelay, () =>
                {
                    if (_parryCoroutine != null) StopCoroutine(_parryCoroutine);


                    rseOnAnimationBoolValueChange.Call(_parryParam, false);
                    _onPlayerAddState.Call(PlayerState.None);

                    
                }));
            }));
        }));
    }

    void CancelParry()
    {
        if (_parryCoroutine == null) return;
        StopCoroutine(_parryCoroutine);

        ResetValue();
    }

    private void ResetValue()
    {
        _canParry.Value = false;
        rseOnAnimationBoolValueChange.Call(_parryParam, false);
    }

}