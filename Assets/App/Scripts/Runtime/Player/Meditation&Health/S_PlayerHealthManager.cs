using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerHealthManager : MonoBehaviour
{
    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerHealPerformed rseOnPlayerHealPerformed;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerHealthReduced rseOnPlayerHealthReduced;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerDeath rseOnPlayerDeath;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerHealthUpdate rseOnPlayerHealthUpdate;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnAnimationBoolValueChange _rseOnAnimationBoolValueChange;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerAddState _onPlayerAddState;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_ConsoleCheats _debugPlayer;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerCurrentHealth rsoPlayerCurrentHealth;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerStats ssoPlayerStats;

    private float maxHealth => ssoPlayerStats.Value.maxHealth;

    private void Awake()
    {
        rsoPlayerCurrentHealth.Value = maxHealth;
    }

    private void OnEnable()
    {
        rseOnPlayerHealPerformed.action += HealPlayer;
        rseOnPlayerHealthReduced.action += ReducePlayerHealth;
    }

    private void OnDisable()
    {
        rseOnPlayerHealPerformed.action -= HealPlayer;
        rseOnPlayerHealthReduced.action -= ReducePlayerHealth;
    }

    private void HealPlayer()
    {
        var newHealth = rsoPlayerCurrentHealth.Value + ssoPlayerStats.Value.healAmount;
        rsoPlayerCurrentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);
        rseOnPlayerHealthUpdate.Call(rsoPlayerCurrentHealth.Value);
    }

    private void ReducePlayerHealth(float damage)
    {
        var newHealth= rsoPlayerCurrentHealth.Value - damage;
        rsoPlayerCurrentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);
        rseOnPlayerHealthUpdate.Call(rsoPlayerCurrentHealth.Value);

        if (rsoPlayerCurrentHealth.Value <= 0)
        {
            if (_debugPlayer.Value.cantDie == true) return;
            rseOnPlayerDeath.Call();
            _rseOnAnimationBoolValueChange.Call("isDead", true);
            _onPlayerAddState.Call(PlayerState.Dying);
        }
    }
}