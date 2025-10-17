using UnityEngine;

public class S_ConvictionManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] SSO_PlayerAttackSteps _playerAttackStep;
    [SerializeField] SSO_PlayerConvictionData _playerConvictionData;
    [SerializeField] SSO_PlayerStats _playerStats;
    [SerializeField] RSO_PlayerCurrentConviction _playerCurrentConviction;

    [Header("Input")]
    [SerializeField] RSE_OnPlayerHealPerformed _onPlayerHealPerformed;
    [SerializeField] RSE_OnPlayerAttackCancel _onPlayerAttackCancel;
    [SerializeField] RSE_OnPlayerGainConviction _onPlayerGainConviction;

    //[Header("Output")]

    private void OnEnable()
    {
        _onPlayerHealPerformed.action += ReduceConvictionOnHealPerformed;

    }

    private void OnDisable()
    {
        _onPlayerHealPerformed.action -= ReduceConvictionOnHealPerformed;
    }

    void ReduceConvictionOnHealPerformed()
    {
        _playerCurrentConviction.Value -= _playerConvictionData.Value.healCost;
    }

    void ReduceConvictionOnAttackCancel(int step)
    {

    }
}