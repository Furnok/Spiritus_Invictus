using UnityEngine;

public class S_PlayerHealthManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SSO_PlayerStats _playerStats;

    [Header("References")]
    [SerializeField] RSO_PlayerCurrentHealth _playerCurrentHealth;

    [Header("Input")]
    [SerializeField] RSE_OnPlayerHealPerformed _onPlayerHealPerformed;
    [SerializeField] RSE_OnPlayerHealthReduced _onPlayerHealthReduced;

    [Header("Output")]
    [SerializeField] RSE_OnPlayerDeath _onPlayerDeath;

    float _maxHealth => _playerStats.Value.maxHealth;

    private void Awake()
    {
        _playerCurrentHealth.Value = _maxHealth;
    }

    void OnEnable()
    {
        _onPlayerHealPerformed.action += HealPlayer;
        _onPlayerHealthReduced.action += ReducePlayerHealth;
    }

    void OnDisable()
    {
        _onPlayerHealPerformed.action -= HealPlayer;
        _onPlayerHealthReduced.action -= ReducePlayerHealth;
    }

    void HealPlayer()
    {
        var newHealth = _playerCurrentHealth.Value + _playerStats.Value.healAmount;
        _playerCurrentHealth.Value = Mathf.Clamp(newHealth, 0, _maxHealth);
        Debug.Log($"Player Healed, player health: {_playerCurrentHealth.Value}");
    }

    void ReducePlayerHealth(float damage)
    {
        var newHealth= _playerCurrentHealth.Value - damage;
        _playerCurrentHealth.Value = Mathf.Clamp(newHealth, 0, _maxHealth);
        Debug.Log($"Player Health: {_playerCurrentHealth.Value}");


        if (_playerCurrentHealth.Value <= 0)
        {
            _onPlayerDeath.Call();
        }
    }
}