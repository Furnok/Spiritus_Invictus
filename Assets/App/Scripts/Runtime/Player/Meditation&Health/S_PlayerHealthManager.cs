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
    [SerializeField] private RSE_OnPlayerHealthUpdate rseOnPlayerHealthUpdate;
    [SerializeField] RSO_ConsoleCheats _debugPlayer;
    [SerializeField] RSE_OnAnimationBoolValueChange _rseOnAnimationBoolValueChange;
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;


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
        //Debug.Log($"Player Healed, player health: {rsoPlayerCurrentHealth.Value}");
    }

    private void ReducePlayerHealth(float damage)
    {
        var newHealth= rsoPlayerCurrentHealth.Value - damage;
        rsoPlayerCurrentHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);
        rseOnPlayerHealthUpdate.Call(rsoPlayerCurrentHealth.Value);
        //Debug.Log($"Player heal reduced, Player Health: {rsoPlayerCurrentHealth.Value}");

        if (rsoPlayerCurrentHealth.Value <= 0)
        {
            if (_debugPlayer.Value.cantDie == true) return;
            rseOnPlayerDeath.Call();
            _rseOnAnimationBoolValueChange.Call("isDead", true);
            _onPlayerAddState.Call(PlayerState.Dying);
        }
    }
}