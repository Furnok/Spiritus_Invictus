using System.Linq;
using Unity.VisualScripting;
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
    [SerializeField] RSE_OnAttackStartPerformed _onAttackStartPerformed;

    Coroutine _attackCoroutine;
    float _timeInputPressed;
    int _currenStepAttack;

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


        _currenStepAttack = 0;
    }

    private void OnEnable()
    {
        rseOnPlayerAttack.action += OnPlayerAttackInput;
        _rseOnPlayerGettingHit.action += CancelAttack;
        _onPlayerAttackInputCancel.action += OnPlayerAttackInputCancel;

        _timeInputPressed = 0;
        _currenStepAttack = 0;

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

        _onAttackStartPerformed.Call();
        rseOnAnimationBoolValueChange.Call(_attackParam, true);

        _timeInputPressed = Time.time;

        //_attackCoroutine = StartCoroutine(S_Utils.Delay(_animationTransitionDelays.Value.attackStartupDelay, () =>
        //{
        if (CanGoUpperState() == true)
        {
            StartStepDuration();
        }
        //}));
    }

    private void StartStepDuration()
    {
        float timeWait = _playerAttackSteps.Value.Find(x => x.step == _currenStepAttack + 1).timeHoldingInput - _playerAttackSteps.Value.Find(x => x.step == _currenStepAttack).timeHoldingInput;
        
        _attackCoroutine = StartCoroutine(S_Utils.Delay(timeWait, () =>
        {
            _currenStepAttack++;

            if (CanGoUpperState() == true)
            {
                StartStepDuration();
            }
        }));
    }

    bool CanGoUpperState()
    {
        var lastStep = _playerAttackSteps.Value.OrderByDescending(x => x.step).First().step;
        bool existNextStep = _playerAttackSteps.Value.Exists(x => x.step == _currenStepAttack + 1);
        bool canGoUpperState = false;

        if (existNextStep == true)
        {
            var nextStep = _playerAttackSteps.Value.Find(x => x.step == _currenStepAttack + 1);
            canGoUpperState = _playerCurrentConviction.Value >= nextStep.ammountConvitionNeeded;

            if (_currenStepAttack < lastStep && canGoUpperState == true)
            {
                return true;
            }
        }

        return false;
    }

    void OnPlayerAttackInputCancel()
    {
        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.Attacking) == false) return;

        var inputHoldDuration = Time.time - _timeInputPressed;

        //if (inputHoldDuration > 0)
        //{
        //    //var currentConviction = _playerCurrentConviction.Value;
        //    //var stepsUpperCurrentConviction = _playerAttackSteps.Value.FindAll(x => x.timeHoldingInput <= inputHoldDuration);
        //    //var currentStep = stepsUpperCurrentConviction.OrderByDescending(x => x.timeHoldingInput).First().step;



        //}

        rseOnSpawnProjectile.Call(_currenStepAttack);

        _timeInputPressed = 0;
        _currenStepAttack = 0;

        if (_attackCoroutine == null) return;
        StopCoroutine(_attackCoroutine);

        _attackCoroutine = StartCoroutine(S_Utils.Delay(_animationTransitionDelays.Value.attackRecoveryDelay, () =>
        {
            _onPlayerAddState.Call(PlayerState.None);
            rseOnAnimationBoolValueChange.Call(_attackParam, false);

            if (_attackCoroutine == null) return;
            StopCoroutine(_attackCoroutine);
        }));
    }

    void CancelAttack()
    {
        if ( _attackCoroutine == null ) return;
        StopCoroutine(_attackCoroutine);
        rseOnAnimationBoolValueChange.Call(_attackParam, false);
       

        var currentConviction = _playerCurrentConviction.Value;
        var stepsUpperCurrentConviction = _playerAttackSteps.Value.FindAll(x => x.ammountConvitionNeeded <= currentConviction);
        var currentStep = stepsUpperCurrentConviction.OrderByDescending(x => x.ammountConvitionNeeded).First().step;

        if(_timeInputPressed > 0)
        {
            _onPlayerAttackCancel.Call(currentStep);
        }

        _timeInputPressed = 0;
        _currenStepAttack = 0;
    }
}

