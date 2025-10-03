using UnityEngine;

public class S_PlayerDamageSystem : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnPlayerTakeDamage rseOnPlayerTakeDamage;

    [Header("Output")]
    [SerializeField] private RSE_OnPlayerHealthReduced rseOnPlayerHealthReduced;

    private void OnEnable()
    {
        rseOnPlayerTakeDamage.action += TakeDamage;
    }
    
    private void OnDisable()
    {
        rseOnPlayerTakeDamage.action -= TakeDamage;
    }

    private void TakeDamage(float damage)
    {
        rseOnPlayerHealthReduced.Call(damage);
        Debug.Log($"Player Took Damage: {damage}");
    }

}