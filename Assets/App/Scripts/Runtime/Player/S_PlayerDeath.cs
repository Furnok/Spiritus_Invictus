using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerDeath : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] private GameObject _playerHurtBoxCollider;

    [TabGroup("References")]
    [Title("Aim Point")]
    [SerializeField] private GameObject _aimPointObject;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerDeath _onPlayerDeathRse;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerRespawn _onPlayerRespawnRse;

    private void OnEnable()
    {
        _onPlayerDeathRse.action += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        _onPlayerDeathRse.action -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        _playerHurtBoxCollider.SetActive(false);
        _aimPointObject.SetActive(false);
    }
}