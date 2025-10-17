using UnityEngine;

public class S_ConvictionManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] SSO_PlayerAttackSteps _playerAttackSteps;
    [SerializeField] SSO_PlayerConvictionData _playerConvictionData;
    [SerializeField] SSO_PlayerStats _playerStats;
    [SerializeField] RSO_PlayerCurrentConviction _playerCurrentConviction;

    [Header("Input")]
    [SerializeField] RSE_OnPlayerHealPerformed _onPlayerHealPerformed;
    [SerializeField] RSE_OnPlayerAttackCancel _onPlayerAttackCancel;
    [SerializeField] RSE_OnPlayerGainConviction _onPlayerGainConviction;

    //[Header("Output")]

    private void Awake()
    {
        _playerCurrentConviction.Value = _playerConvictionData.Value.maxConviction;
    }

    private void OnEnable()
    {
        _onPlayerHealPerformed.action += ReduceConvictionOnHealPerformed;
        _onPlayerAttackCancel.action += ReduceConvictionOnAttackCancel;
        _onPlayerGainConviction.action += OnPlayerGainConviction;

    }

    private void OnDisable()
    {
        _onPlayerHealPerformed.action -= ReduceConvictionOnHealPerformed;
        _onPlayerAttackCancel.action -= ReduceConvictionOnAttackCancel;
        _onPlayerGainConviction.action -= OnPlayerGainConviction;
    }

    void ReduceConvictionOnHealPerformed()
    {
        var ammount = Mathf.Clamp(_playerCurrentConviction.Value - _playerConvictionData.Value.healCost, 0, _playerConvictionData.Value.maxConviction);
        _playerCurrentConviction.Value = ammount;
    }

    void ReduceConvictionOnAttackCancel(int stepCancel)
    {
        if (stepCancel == 0) return;

        var currentStep = _playerAttackSteps.Value.Find(x => x.step == stepCancel);

        if (currentStep.step == stepCancel)
        {
            Debug.LogError("Didn't find the step");
            return;
        }

        var stepUnder = _playerAttackSteps.Value.Find(x => x.step == currentStep.step - 1);
        var stepUpper = _playerAttackSteps.Value.Find(x => x.step == currentStep.step + 1);

        var differenceWithUpper = Mathf.Abs(stepUpper.ammountConvitionNeeded - currentStep.ammountConvitionNeeded);
        var percentage = (_playerCurrentConviction.Value - currentStep.ammountConvitionNeeded) * 100 / differenceWithUpper;

        var differenceWithUnder = Mathf.Abs(stepUnder.ammountConvitionNeeded - currentStep.ammountConvitionNeeded);

        var newConvictionValue = stepUnder.ammountConvitionNeeded + differenceWithUnder / 100 * percentage;

        _playerCurrentConviction.Value = newConvictionValue;

    }

    void OnPlayerGainConviction(float ammountGain)
    {
        var ammount = Mathf.Clamp(ammountGain + _playerCurrentConviction.Value, 0, _playerConvictionData.Value.maxConviction);
        _playerCurrentConviction.Value = ammount;
    }
}