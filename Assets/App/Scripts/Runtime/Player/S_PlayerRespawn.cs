using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerRespawn : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Animation")]
    [SerializeField, S_AnimationName] private string _deadParam;

    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] private GameObject _playerHurtBoxCollider;

    [TabGroup("References")]
    [Title("Aim Point")]
    [SerializeField] private GameObject _aimPointObject;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerRespawn _onPlayerRespawnRse;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerAddState _onPlayerAddStateRse;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnAnimationBoolValueChange _onAnimationBoolValueChange;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerHealthUpdate _onPlayerHealthUpdate;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnPlayerConvictionUpdate _onPlayerConvictionUpdate;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerRespawnPosition _playerRespawnPosition;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerCurrentHealth _playerCurrentHealth;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PlayerCurrentConviction _playerCurrentConviction;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerStats _playerStats;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerConvictionData _playerConvictionData;
   
    private void OnEnable()
    {
        _onPlayerRespawnRse.action += HandlePlayerRespawn;
    }

    private void OnDisable()
    {
        _onPlayerRespawnRse.action -= HandlePlayerRespawn;

        // Reset respawn position for now change it afterwards
        _playerRespawnPosition.Value.rotation = Quaternion.identity;
    }

    private void HandlePlayerRespawn()
    {
        _playerHurtBoxCollider.SetActive(true);
        _aimPointObject.SetActive(true);

        transform.SetPositionAndRotation(_playerRespawnPosition.Value.position, _playerRespawnPosition.Value.rotation);

        _onAnimationBoolValueChange.Call(_deadParam, false);
        _onPlayerAddStateRse.Call(PlayerState.None);

        _playerCurrentHealth.Value = _playerStats.Value.maxHealth;
        _onPlayerHealthUpdate.Call(_playerCurrentHealth.Value);

        _playerCurrentConviction.Value = _playerConvictionData.Value.startConviction;
        _onPlayerConvictionUpdate.Call(_playerCurrentConviction.Value);
    }
}