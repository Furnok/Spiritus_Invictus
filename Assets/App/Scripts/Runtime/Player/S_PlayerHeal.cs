using UnityEngine;

public class S_PlayerHeal : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] SSO_PlayerStats _playerStats;

    [Header("Input")]
    [SerializeField] RSE_OnPlayerHealInput _onPlayerHeal;
    [SerializeField] RSE_OnPlayerGettingHit _onPlayerGettingHit;

    [Header("Output")]
    [SerializeField] RSE_OnPlayerHealPerformed _onPlayerHealPerformed;

    Coroutine _healCoroutine;

    private void OnEnable()
    {
        _onPlayerHeal.action += TryHeal;
        _onPlayerGettingHit.action += CancelHeal;
    }

    private void OnDisable()
    {
        _onPlayerHeal.action -= TryHeal;
        _onPlayerGettingHit.action -= CancelHeal;
    }

    void TryHeal()
    {
        // Implement can heal logic here (cost conviction and if he can during the current action)

        _healCoroutine = StartCoroutine(S_Utils.Delay(_playerStats.Value.delayBeforeHeal, () =>
        {
            _onPlayerHealPerformed.Call();
        }));


        Debug.Log("Player Heal");
    }

    void CancelHeal()
    {
        if(_healCoroutine != null)
        {
            StopCoroutine(_healCoroutine);
        }
    }
}