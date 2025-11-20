using UnityEngine;

public class S_ConvictionManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] SSO_PlayerAttackSteps _playerAttackSteps;
    [SerializeField] SSO_PlayerConvictionData _playerConvictionData;
    [SerializeField] SSO_PlayerStats _playerStats;
    [SerializeField] RSO_PlayerCurrentConviction _playerCurrentConviction;
    [SerializeField] RSO_ConsoleCheats _debugPlayer;

    [Header("Input")]
    [SerializeField] RSE_OnHealStart _onHealStart;
    [SerializeField] RSE_OnPlayerAttackCancel _onPlayerAttackCancel;
    [SerializeField] RSE_OnPlayerGainConviction _onPlayerGainConviction;
    [SerializeField] RSE_OnSpawnProjectile _onSpawnProjectile;
    [SerializeField] RSE_OnAttackStartPerformed _onAttackStartPerformed;

    [Header("Output")]
    [SerializeField] private RSE_OnPlayerConvictionUpdate rseOnPlayerConvictionUpdate;

    Coroutine _convictionConsumptionCoroutine;
    Coroutine _convictionGainOrLossCoroutine;

    private void Awake()
    {
        _playerCurrentConviction.Value = _playerConvictionData.Value.startConviction;
        StartConvitionConsumption(); //change it when the player are playing and not pause the game
    }

    private void OnEnable()
    {
        _onHealStart.action += ReduceConvictionOnHealPerformed;
        _onPlayerAttackCancel.action += ReduceConvictionOnAttackCancel;
        _onPlayerGainConviction.action += OnPlayerGainConviction;
        _onSpawnProjectile.action += ReductionConviction;
        _onAttackStartPerformed.action += StopComsuptioncoroutine;
    }

    private void OnDisable()
    {
        _onHealStart.action -= ReduceConvictionOnHealPerformed;
        _onPlayerAttackCancel.action -= ReduceConvictionOnAttackCancel;
        _onPlayerGainConviction.action -= OnPlayerGainConviction;
        _onSpawnProjectile.action -= ReductionConviction;
        _onAttackStartPerformed.action -= StopComsuptioncoroutine;
    }

    void ReduceConvictionOnHealPerformed()
    {
        var newAmmount = Mathf.Clamp(_playerCurrentConviction.Value - _playerConvictionData.Value.healCost, 0, _playerConvictionData.Value.maxConviction);
        _playerCurrentConviction.Value = newAmmount;
        rseOnPlayerConvictionUpdate.Call(newAmmount);

        if (_debugPlayer.Value.infiniteConviction == true)
        {
            StartCoroutine(S_Utils.Delay(0.3f, () =>
            {
                _playerCurrentConviction.Value = _playerConvictionData.Value.maxConviction;
                rseOnPlayerConvictionUpdate.Call(_playerConvictionData.Value.maxConviction);
            }));

            return;
        }

        DelayWhenConvictionLoss();
    }

    void ReduceConvictionOnAttackCancel(int stepCancel)
    {
        if (_debugPlayer.Value.infiniteConviction == true) return;
        if (stepCancel == 0) return;

        var currentStep = _playerAttackSteps.Value.Find(x => x.step == stepCancel);

        if (currentStep.step != stepCancel)
        {
            Debug.LogError($"Didn't find the step {currentStep.step} & {stepCancel}");
            return;
        }

        var stepUnder = _playerAttackSteps.Value.Find(x => x.step == currentStep.step - 1);
        var stepUpper = _playerAttackSteps.Value.Find(x => x.step == currentStep.step + 1);

        var differenceWithUpper = Mathf.Abs(stepUpper.ammountConvitionNeeded - currentStep.ammountConvitionNeeded);
        var percentage = (_playerCurrentConviction.Value - currentStep.ammountConvitionNeeded) * 100 / differenceWithUpper;

        var differenceWithUnder = Mathf.Abs(stepUnder.ammountConvitionNeeded - currentStep.ammountConvitionNeeded);

        var newConvictionValue = stepUnder.ammountConvitionNeeded + differenceWithUnder / 100 * percentage;

        _playerCurrentConviction.Value = newConvictionValue;
        rseOnPlayerConvictionUpdate.Call(newConvictionValue);

        DelayWhenConvictionLoss();
    }

    //void ReduceConvitionOnAttackPerform(int attaqueStep)
    //{
    //    var step = _playerAttackSteps.Value.Find(x => x.step == attaqueStep);

    //    var newAmmount = Mathf.Clamp(_playerCurrentConviction.Value - step.ammountConvitionNeeded, 0, _playerConvictionData.Value.maxConviction);

    //    if (step.ammountConvitionNeeded != 0)
    //    {
    //        _playerCurrentConviction.Value = newAmmount;
    //        rseOnPlayerConvictionUpdate.Call(newAmmount);

    //        DelayWhenConvictionLoss();
    //    }
    //    else
    //    {
    //        StartConvitionConsumption();
    //    }

    //    DelayWhenConvictionLoss();

    //}

    void OnPlayerGainConviction(float ammountGain)
    {
        var ammount = Mathf.Clamp(ammountGain + _playerCurrentConviction.Value, 0, _playerConvictionData.Value.maxConviction);
        _playerCurrentConviction.Value = ammount;
        rseOnPlayerConvictionUpdate.Call(ammount);

        DelayWhenConvictionGain();
    }

    void StartConvitionConsumption()
    {
        if (_debugPlayer.Value.infiniteConviction == true) return;
        StopComsuptioncoroutine();

        _convictionConsumptionCoroutine = StartCoroutine(S_Utils.Delay(_playerConvictionData.Value.tickIntervalSec, () =>
        {
            ReductionConsumptionOnConsuption(_playerConvictionData.Value.ammountLostOverTick);
            StartConvitionConsumption();
        }));
    }

    void ReductionConviction(float ammount)
    {
        var newAmmount = Mathf.Clamp(_playerCurrentConviction.Value - ammount, 0, _playerConvictionData.Value.maxConviction);
        _playerCurrentConviction.Value = newAmmount;
        rseOnPlayerConvictionUpdate.Call(newAmmount);

        if (_debugPlayer.Value.infiniteConviction == true)
        {
            StartCoroutine(S_Utils.Delay(0.3f, () =>
            {
                _playerCurrentConviction.Value = _playerConvictionData.Value.maxConviction;
                rseOnPlayerConvictionUpdate.Call(_playerConvictionData.Value.maxConviction);
            }));
            
            return;
        }

        if (ammount >= 1)
        {
            DelayWhenConvictionLoss();
        }
        else
        {
            StartConvitionConsumption();
        }
    }

    void ReductionConsumptionOnConsuption(float ammount)
    {
        var newAmmount = Mathf.Clamp(_playerCurrentConviction.Value - ammount, 0, _playerConvictionData.Value.maxConviction);
        _playerCurrentConviction.Value = newAmmount;
        rseOnPlayerConvictionUpdate.Call(newAmmount);
    }

    void DelayWhenConvictionLoss()
    {
        StopComsuptioncoroutine();
        if(_convictionGainOrLossCoroutine != null) StopCoroutine(_convictionGainOrLossCoroutine);

        _convictionGainOrLossCoroutine = StartCoroutine(S_Utils.Delay(_playerConvictionData.Value.pauseIntervalAfterLoss, () =>
        {
            if (_playerCurrentConviction.Value > 0)
            {
                StartConvitionConsumption();
            }
        }));
    }

    void DelayWhenConvictionGain()
    {
        StopComsuptioncoroutine();
        if(_convictionGainOrLossCoroutine != null) StopCoroutine(_convictionGainOrLossCoroutine);

        _convictionGainOrLossCoroutine = StartCoroutine(S_Utils.Delay(_playerConvictionData.Value.pauseIntervalAfterGained, () =>
        {
            StartConvitionConsumption();
        }));
    }

    void StopComsuptioncoroutine()
    {
        if (_convictionConsumptionCoroutine != null)
        {
            StopCoroutine(_convictionConsumptionCoroutine);
        }
    }
}