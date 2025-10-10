using UnityEngine;

public class S_PlayerDamageSystem : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnPlayerTakeDamage rseOnPlayerTakeDamage;
    [SerializeField] RSE_OnPlayerHit _rseOnPlayerHit;


    [Header("Output")]
    [SerializeField] private RSE_OnPlayerHealthReduced rseOnPlayerHealthReduced;

    private void OnEnable()
    {
        rseOnPlayerTakeDamage.action += TakeDamage;
        _rseOnPlayerHit.action += TakeDamage;
    }
    
    private void OnDisable()
    {
        rseOnPlayerTakeDamage.action -= TakeDamage;
        _rseOnPlayerHit.action -= TakeDamage;

    }

    private void TakeDamage(float damage)
    {
        rseOnPlayerHealthReduced.Call(damage);
        Debug.Log($"Player Took Damage: {damage}");
    }


    private void TakeDamage(AttackData attackData)
    {
        rseOnPlayerHealthReduced.Call(attackData.damage);
    }
}