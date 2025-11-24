using UnityEngine;

public class S_PlayerDeath : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] GameObject _playerHurtBoxCollider;
    [SerializeField] GameObject _aimPointObject;

    [Header("Inputs")]
    [SerializeField] RSE_OnPlayerDeath _onPlayerDeathRse;
    [SerializeField] RSE_OnPlayerRespawn _onPlayerRespawnRse;

    //[Header("Outputs")]

    private void OnEnable()
    {
        _onPlayerDeathRse.action += HandlePlayerDeath;
        _onPlayerRespawnRse.action += HandlePlayerRespawn;
    }

    void OnDisable()
    {
        _onPlayerDeathRse.action -= HandlePlayerDeath;
        _onPlayerRespawnRse.action -= HandlePlayerRespawn;
    }

    private void HandlePlayerDeath()
    {
        _playerHurtBoxCollider.SetActive(false);
        _aimPointObject.SetActive(false);
    }

    private void HandlePlayerRespawn()
    {
        _playerHurtBoxCollider.SetActive(true);
        _aimPointObject.SetActive(true);
    }
}