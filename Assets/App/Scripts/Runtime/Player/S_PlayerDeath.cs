using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerDeath : MonoBehaviour
{
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
    [SerializeField] GameObject _visuals;

    [TabGroup("References")]
    [SerializeField] GameObject _player;

    [TabGroup("References")]
    [SerializeField] GameObject _colliderMotorGO;

    [TabGroup("References")]
    [SerializeField] Collider _colliderMotor;

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

        _playerRigidbody.linearVelocity = Vector3.zero;

        _colliderMotor.providesContacts = false;
        _colliderMotor.enabled = false;

        _colliderMotorGO.SetActive(false);

        _playerRigidbody.useGravity = false;

        Physics.SyncTransforms();
    }
}