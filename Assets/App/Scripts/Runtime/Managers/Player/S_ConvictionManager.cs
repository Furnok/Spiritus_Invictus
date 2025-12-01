using Sirenix.OdinInspector;
using UnityEngine;

public class S_ConvictionManager : MonoBehaviour
{
    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnHealStart _onHealStart;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerAttackCancel _onPlayerAttackCancel;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerGainConviction _onPlayerGainConviction;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnSpawnProjectile _onSpawnProjectile;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnAttackStartPerformed _onAttackStartPerformed;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerConvictionUpdate rseOnPlayerConvictionUpdate;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerAttackSteps _playerAttackSteps;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerConvictionData _playerConvictionData;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerStats _playerStats;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerCurrentConviction _playerCurrentConviction;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_ConsoleCheats _debugPlayer;

    private Coroutine _convictionConsumptionCoroutine = null;
    private Coroutine _convictionGainOrLossCoroutine = null;

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

    private void ReduceConvictionOnHealPerformed()
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

    private void ReduceConvictionOnAttackCancel(int stepCancel)
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

    /*
    private void ReduceConvitionOnAttackPerform(int attaqueStep)
    {
        var step = _playerAttackSteps.Value.Find(x => x.step == attaqueStep);

        var newAmmount = Mathf.Clamp(_playerCurrentConviction.Value - step.ammountConvitionNeeded, 0, _playerConvictionData.Value.maxConviction);

        if (step.ammountConvitionNeeded != 0)
        {
            _playerCurrentConviction.Value = newAmmount;
            rseOnPlayerConvictionUpdate.Call(newAmmount);

            DelayWhenConvictionLoss();
        }
        else
        {
            StartConvitionConsumption();
        }

        DelayWhenConvictionLoss();
    }
    */

    private void OnPlayerGainConviction(float ammountGain)
    {
        var ammount = Mathf.Clamp(ammountGain + _playerCurrentConviction.Value, 0, _playerConvictionData.Value.maxConviction);
        _playerCurrentConviction.Value = ammount;
        rseOnPlayerConvictionUpdate.Call(ammount);

        DelayWhenConvictionGain();
    }

    private void StartConvitionConsumption()
    {
        if (_debugPlayer.Value.infiniteConviction == true) return;
        StopComsuptioncoroutine();

        _convictionConsumptionCoroutine = StartCoroutine(S_Utils.Delay(_playerConvictionData.Value.tickIntervalSec, () =>
        {
            ReductionConsumptionOnConsuption(_playerConvictionData.Value.ammountLostOverTick);
            StartConvitionConsumption();
        }));
    }

    private void ReductionConviction(float ammount)
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

    private void ReductionConsumptionOnConsuption(float ammount)
    {
        var newAmmount = Mathf.Clamp(_playerCurrentConviction.Value - ammount, 0, _playerConvictionData.Value.maxConviction);
        _playerCurrentConviction.Value = newAmmount;
        rseOnPlayerConvictionUpdate.Call(newAmmount);
    }

    private void DelayWhenConvictionLoss()
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

    private void DelayWhenConvictionGain()
    {
        StopComsuptioncoroutine();
        if(_convictionGainOrLossCoroutine != null) StopCoroutine(_convictionGainOrLossCoroutine);

        _convictionGainOrLossCoroutine = StartCoroutine(S_Utils.Delay(_playerConvictionData.Value.pauseIntervalAfterGained, () =>
        {
            StartConvitionConsumption();
        }));
    }

    private void StopComsuptioncoroutine()
    {
        if (_convictionConsumptionCoroutine != null)
        {
            StopCoroutine(_convictionConsumptionCoroutine);
        }
    }
}