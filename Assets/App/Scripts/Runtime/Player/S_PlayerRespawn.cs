using UnityEngine;

public class S_PlayerRespawn : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] GameObject _playerHurtBoxCollider;
    [SerializeField] GameObject _aimPointObject;
    [SerializeField] RSO_PlayerRespawnPosition _playerRespawnPosition;
    [SerializeField] SSO_PlayerStats _playerStats;
    [SerializeField] RSO_PlayerCurrentHealth _playerCurrentHealth;
    [SerializeField] SSO_PlayerConvictionData _playerConvictionData;
    [SerializeField] RSO_PlayerCurrentConviction _playerCurrentConviction;

    [Header("Inputs")]
    [SerializeField] RSE_OnPlayerRespawn _onPlayerRespawnRse;

    [Header("Outputs")]
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddStateRse;
    [SerializeField] RSE_OnAnimationBoolValueChange _onAnimationBoolValueChange;
    [SerializeField] RSE_OnPlayerHealthUpdate _onPlayerHealthUpdate;
    [SerializeField] RSE_OnPlayerConvictionUpdate _onPlayerConvictionUpdate;


    private void OnEnable()
    {
        _onPlayerRespawnRse.action += HandlePlayerRespawn;
    }

    void OnDisable()
    {
        _onPlayerRespawnRse.action -= HandlePlayerRespawn;

        // Reset respawn position for now change it afterwards
        _playerRespawnPosition.Value.position = Vector3.zero;
        _playerRespawnPosition.Value.rotation = Quaternion.identity;
    }


    private void HandlePlayerRespawn()
    {
        _playerHurtBoxCollider.SetActive(true);
        _aimPointObject.SetActive(true);

        transform.SetPositionAndRotation(_playerRespawnPosition.Value.position, _playerRespawnPosition.Value.rotation);

        _onAnimationBoolValueChange.Call("isDead", false);
        _onPlayerAddStateRse.Call(PlayerState.None);

        _playerCurrentHealth.Value = _playerStats.Value.maxHealth;
        _onPlayerHealthUpdate.Call(_playerCurrentHealth.Value);

        _playerCurrentConviction.Value = _playerConvictionData.Value.startConviction;
        _onPlayerConvictionUpdate.Call(_playerCurrentConviction.Value);
    }
}