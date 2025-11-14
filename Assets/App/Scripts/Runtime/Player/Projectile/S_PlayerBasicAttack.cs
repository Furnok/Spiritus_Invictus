using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] SSO_AnimationTransitionDelays _animationTransitionDelays;
    [SerializeField] RSO_PreconsumedConviction _preconsumedConviction;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerAttackInput rseOnPlayerAttack;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;
   
    [SerializeField] RSE_OnPlayerAttackInputCancel _onPlayerAttackInputCancel;

    [Header("Output")]
    [SerializeField] private RSE_OnSpawnProjectile rseOnSpawnProjectile;
    [SerializeField] private RSO_PlayerIsTargeting rsoPlayerIsTargeting;
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;
    [SerializeField] RSE_OnPlayerAttackCancel _onPlayerAttackCancel;
    [SerializeField] RSE_OnAttackStartPerformed _onAttackStartPerformed;

    Coroutine _attackChargeCoroutine;

    bool _isHolding;
    bool _wasCanceled;

    List<PlayerAttackStep> _steps;
    private List<float> _stepTimes = new List<float>();
    private List<float> _stepConvThresholds = new List<float>();
    float _reservedConviction;
    int _lastCompletedStep;

    private void Awake()
    {
        _preconsumedConviction.Value = 0;

        _steps = _playerAttackSteps.Value?.OrderBy(s => s.step).ToList() ?? new List<PlayerAttackStep>();
        if (_steps.Count == 0)
        {
            Debug.LogWarning("No attack steps configured");
            return;
        }

        var dup = _steps.GroupBy(s => s.step).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
        if (dup.Count > 0) Debug.LogError("There is duplicate step in the attack steps SSO ");

        foreach (var s in _steps)
        {
            _stepTimes.Add(s.timeHoldingInput);
            _stepConvThresholds.Add(s.ammountConvitionNeeded);
        }
    }

    private void OnEnable()
    {
        rseOnPlayerAttack.action += OnPlayerAttackInput;
        _rseOnPlayerGettingHit.action += OnHitCancel;
        _onPlayerAttackInputCancel.action += OnAttackReleased;
    }

    private void OnDisable()
    {
        rseOnPlayerAttack.action -= OnPlayerAttackInput;
        _rseOnPlayerGettingHit.action -= OnHitCancel;
        _onPlayerAttackInputCancel.action -= OnAttackReleased;
    }

    private void OnPlayerAttackInput()
    {
        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Attacking) == false ||_playerCurrentConviction.Value < 2) return;

        _onPlayerAddState.Call(PlayerState.Attacking);
        _onAttackStartPerformed.Call();
        rseOnAnimationBoolValueChange.Call(_attackParam, true);

        //_attackCoroutine = StartCoroutine(S_Utils.Delay(_animationTransitionDelays.Value.attackStartupDelay, () =>
        //{
        //if (CanGoUpperState() == true)
        //{
        //    StartStepDuration();
        //}
        //}));

        _isHolding = true;
        _wasCanceled = false;
        _reservedConviction = 0f;
        _lastCompletedStep = 0;
        PublishPreconsume(_reservedConviction);

        if (_attackChargeCoroutine != null) StopCoroutine(_attackChargeCoroutine);
        _attackChargeCoroutine = StartCoroutine(ChargeRoutine());
    }

    void OnAttackReleased()
    {
        _isHolding = false;
        rseOnAnimationBoolValueChange.Call(_attackParam, false);
    }

    //private void StartStepDuration()
    //{
    //    float timeWait = _playerAttackSteps.Value.Find(x => x.step == _currenStepAttack + 1).timeHoldingInput - _playerAttackSteps.Value.Find(x => x.step == _currenStepAttack).timeHoldingInput;

    //    _attackChargeCoroutine = StartCoroutine(S_Utils.Delay(timeWait, () =>
    //    {
    //        _currenStepAttack++;

    //        if (CanGoUpperState() == true)
    //        {
    //            StartStepDuration();
    //        }
    //    }));
    //}

    //bool CanGoUpperState()
    //{
    //    var lastStep = _playerAttackSteps.Value.OrderByDescending(x => x.step).First().step;
    //    bool existNextStep = _playerAttackSteps.Value.Exists(x => x.step == _currenStepAttack + 1);
    //    bool canGoUpperState = false;

    //    if (existNextStep == true)
    //    {
    //        var nextStep = _playerAttackSteps.Value.Find(x => x.step == _currenStepAttack + 1);
    //        canGoUpperState = _playerCurrentConviction.Value >= nextStep.ammountConvitionNeeded;

    //        if (_currenStepAttack < lastStep && canGoUpperState == true)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    IEnumerator ChargeRoutine()
    {
        yield return new WaitForSeconds(_animationTransitionDelays.Value.attackStartupDelay);

        float started = Time.time;
        float pauseCarry = 0f;

        // i go in each attackSteps
        for (int i = 1; i < _stepTimes.Count; i++)
        {
            if (!_isHolding || _wasCanceled) break;

            float tStart = _stepTimes[i - 1];
            float tEnd = _stepTimes[i];

            float convStart = _stepConvThresholds[i - 1];
            float convEnd = _stepConvThresholds[i];

            float cap = _playerCurrentConviction.Value;
            float targetEnd = Mathf.Min(convEnd, cap);

            if (targetEnd <= convStart + 0.0001f)
            {
                _reservedConviction = Mathf.Min(cap, Mathf.Max(_reservedConviction, convStart));
                PublishPreconsume(_reservedConviction);
                while (_isHolding && !_wasCanceled) yield return null;
                break;
            }

            while (_isHolding && !_wasCanceled && _reservedConviction < targetEnd - 0.0001f)
            {
                float elapsed = (Time.time - started - pauseCarry);
                float segT = Mathf.InverseLerp(tStart, tEnd, Mathf.Clamp(elapsed, tStart, tEnd));

                _reservedConviction = Mathf.Lerp(convStart, targetEnd, segT);
                PublishPreconsume(_reservedConviction);

                yield return null;
            }

            if (!_isHolding || _wasCanceled) break;

            _reservedConviction = Mathf.Min(_reservedConviction, targetEnd);
            PublishPreconsume(_reservedConviction);

            bool fullStepReached = Mathf.Abs(_reservedConviction - convEnd) <= 0.0001f;
            if (fullStepReached)
            {
                _lastCompletedStep = i;

                int lastIndex = _stepTimes.Count - 1;
                bool isFirstBoundaryFromZero = i == 1 && Mathf.Approximately(_stepTimes[0], 0f) && Mathf.Approximately(_stepConvThresholds[0], 0f);

                bool isLastBoundary = (i == lastIndex);

                float pause = (/*!isFirstBoundaryFromZero && */!isLastBoundary) ? Mathf.Max(0f, _playerStats.Value.timeWaitBetweenSteps) : 0f;

                if (pause > 0f)
                {
                    float endPause = Time.time + pause;
                    while (Time.time < endPause)
                    {
                        if (!_isHolding || _wasCanceled) break;
                        yield return null;
                    }
                    pauseCarry += pause;
                }
            }

            if (_reservedConviction >= cap - 0.0001f)
            {
                while (_isHolding && !_wasCanceled) yield return null;
                break;
            }
        }

        if (!_wasCanceled)
        {
            FinalizeAttack();
        }
    }

    private void FinalizeAttack()
    {
        _attackChargeCoroutine = StartCoroutine(S_Utils.Delay(_playerStats.Value.delayBeforeCastAttack, () =>
        {
            rseOnAnimationBoolValueChange.Call(_attackParam, false);
            var value = Mathf.FloorToInt(_reservedConviction);

            rseOnSpawnProjectile.Call(value);

            _reservedConviction = 0f;
            PublishPreconsume(_reservedConviction);

            _attackChargeCoroutine = StartCoroutine(S_Utils.Delay(_animationTransitionDelays.Value.attackRecoveryDelay, () =>
            {
                _onPlayerAddState.Call(PlayerState.None);
                if (_attackChargeCoroutine == null) return;
                _attackChargeCoroutine = null;
            }));

        }));

    }

    void OnHitCancel()
    {
        var currentConviction = _playerCurrentConviction.Value;
        var stepsUpperCurrentConviction = _playerAttackSteps.Value.FindAll(x => x.ammountConvitionNeeded <= currentConviction);
        var currentStep = stepsUpperCurrentConviction.OrderByDescending(x => x.ammountConvitionNeeded).First().step;

        _wasCanceled = true;
        _isHolding = false;

        if (_attackChargeCoroutine != null)
        {
            StopCoroutine(_attackChargeCoroutine);
            _onPlayerAttackCancel.Call(currentStep);
        }

        rseOnAnimationBoolValueChange.Call(_attackParam, false);

        _reservedConviction = 0f;
        PublishPreconsume(_reservedConviction);
    }


    private void PublishPreconsume(float value)
    {
        if (_preconsumedConviction != null)
        {
            _preconsumedConviction.Value = value;
        }
    }
}

