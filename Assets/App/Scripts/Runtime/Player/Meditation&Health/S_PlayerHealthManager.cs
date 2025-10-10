using UnityEngine;

public class S_PlayerHealthManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnPlayerHealPerformed rseOnPlayerHealPerformed;
    [SerializeField] private RSE_OnPlayerHealthReduced rseOnPlayerHealthReduced;

    [Header("Output")]
    [SerializeField] private SSO_PlayerStats ssoPlayerStats;
    [SerializeField] private RSO_PlayerCurrentHealth rsoPlayerCurrentHealth;
    [SerializeField] private RSE_OnPlayerDeath rseOnPlayerDeath;

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
        //Debug.Log($"Player Healed, player health: {rsoPlayerCurrentHealth.Value}");
    }

    private void ReducePlayerHealth(float damage)
    {
        var newHealth= rsoPlayerCurrentHealth.Value - damage;
        rsoPlayerCurrentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);
        //Debug.Log($"Player heal reduced, Player Health: {rsoPlayerCurrentHealth.Value}");

        if (rsoPlayerCurrentHealth.Value <= 0)
        {
            rseOnPlayerDeath.Call();
        }
    }
}