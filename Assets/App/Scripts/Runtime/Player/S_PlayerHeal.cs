using UnityEngine;

public class S_PlayerHeal : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnPlayerHealInput rseOnPlayerHeal;
    [SerializeField] private RSE_OnPlayerGettingHit rseOnPlayerGettingHit;

    [Header("Output")]
    [SerializeField] private SSO_PlayerStats ssoPlayerStats;
    [SerializeField] private RSE_OnPlayerHealPerformed rseOnPlayerHealPerformed;

    private Coroutine healCoroutine = null;

    private void OnEnable()
    {
        rseOnPlayerHeal.action += TryHeal;
        rseOnPlayerGettingHit.action += CancelHeal;
    }

    private void OnDisable()
    {
        rseOnPlayerHeal.action -= TryHeal;
        rseOnPlayerGettingHit.action -= CancelHeal;
    }

    private void TryHeal()
    {
        healCoroutine = StartCoroutine(S_Utils.Delay(ssoPlayerStats.Value.delayBeforeHeal, () =>
        {
            rseOnPlayerHealPerformed.Call();
        }));
    }

    private void CancelHeal()
    {
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
        }
    }
}