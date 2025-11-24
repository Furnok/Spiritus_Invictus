using UnityEngine;

public class S_PlayerDeath : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] GameObject _playerHurtBoxCollider;
    [SerializeField] GameObject _aimPointObject;

    [Header("Inputs")]
    [SerializeField] RSE_OnPlayerDeath _onPlayerDeathRse;

    [Header("Outputs")]
    [SerializeField] RSE_OnPlayerRespawn _onPlayerRespawnRse;


    private void OnEnable()
    {
        _onPlayerDeathRse.action += HandlePlayerDeath;
    }

    void OnDisable()
    {
        _onPlayerDeathRse.action -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        _playerHurtBoxCollider.SetActive(false);
        _aimPointObject.SetActive(false);

        //StartCoroutine(S_Utils.Delay(2f, RespawnPlayer)); //testing
    }

    //Testing
    void RespawnPlayer()
    {
        _onPlayerRespawnRse.Call();
    }
}