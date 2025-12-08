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
    [Title("Rigidbody")]
    [SerializeField] Rigidbody _playerRigidbody;

    [TabGroup("References")]
    [Title("Aim Point")]
    [SerializeField] private GameObject _aimPointObject;

    [TabGroup("References")]
    [Title("Others")]
    [SerializeField] GameObject _colliderMotor;

    [TabGroup("References")]
    [SerializeField] GameObject _visuals;

    [TabGroup("References")]
    [SerializeField] GameObject _player;

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
        _aimPointObject.SetActive(true);

        _playerRigidbody.linearVelocity = Vector3.zero;

        _player.transform.SetPositionAndRotation(_playerRespawnPosition.Value.position, _playerRespawnPosition.Value.rotation);
        _player.transform.position = _playerRespawnPosition.Value.position;
        _player.transform.rotation = _playerRespawnPosition.Value.rotation;

        //_visuals.transform.localPosition = new Vector3(0, 0, 0);

        StartCoroutine(S_Utils.DelayRealTime(0.1f, () => 
        {
            _playerRigidbody.linearVelocity = Vector3.zero;

            _player.transform.position = _playerRespawnPosition.Value.position;
            _player.transform.rotation = _playerRespawnPosition.Value.rotation;

            _playerHurtBoxCollider.SetActive(true);
            _colliderMotor.SetActive(true);

            _playerRigidbody.useGravity = true;

            _playerRigidbody.linearVelocity = Vector3.zero;
        }));

        _onAnimationBoolValueChange.Call(_deadParam, false);
        _onPlayerAddStateRse.Call(S_EnumPlayerState.None);

        _playerCurrentHealth.Value = _playerStats.Value.maxHealth;
        _onPlayerHealthUpdate.Call(_playerCurrentHealth.Value);

        _playerCurrentConviction.Value = _playerConvictionData.Value.startConviction;
        _onPlayerConvictionUpdate.Call(_playerCurrentConviction.Value);
    }
}