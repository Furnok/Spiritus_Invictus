using UnityEngine;

public class S_PlayerDamageSystem : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnPlayerTakeDamage rseOnPlayerTakeDamage;
    [SerializeField] RSE_OnPlayerHit _rseOnPlayerHit;

    [Header("Output")]
    [SerializeField] private RSE_OnPlayerHealthReduced rseOnPlayerHealthReduced;
    [SerializeField] RSE_OnAnimationTriggerValueChange rseOnAnimationTriggerValueChange;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;

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
        rseOnAnimationTriggerValueChange.Call("isHit");
        rseOnPlayerHealthReduced.Call(damage);
        _rseOnPlayerGettingHit.Call();
        Debug.Log("Getting Hit");
    }


    private void TakeDamage(EnemyAttackData attackData)
    {
        rseOnAnimationTriggerValueChange.Call("isHit");
        rseOnPlayerHealthReduced.Call(attackData.damage);
        _rseOnPlayerGettingHit.Call();

    }
}