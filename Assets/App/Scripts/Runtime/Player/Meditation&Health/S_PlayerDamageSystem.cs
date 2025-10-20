using UnityEngine;

public class S_PlayerDamageSystem : MonoBehaviour
{
    [SerializeField] SSO_PlayerStateTransitions _playerStateTransitions;
    [SerializeField] RSO_PlayerCurrentState _playerCurrentState;
    [SerializeField] SSO_PlayerStats _playerStats;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerTakeDamage rseOnPlayerTakeDamage;
    [SerializeField] RSE_OnPlayerHit _rseOnPlayerHit;

    [Header("Output")]
    [SerializeField] private RSE_OnPlayerHealthReduced rseOnPlayerHealthReduced;
    [SerializeField] RSE_OnAnimationTriggerValueChange rseOnAnimationTriggerValueChange;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;

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
    }


    private void TakeDamage(EnemyAttackData attackData)
    {
        if(_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.HitReact) == true)
        {
            rseOnAnimationTriggerValueChange.Call("isHit");
            _onPlayerAddState.Call(PlayerState.HitReact);

            StartCoroutine(S_Utils.Delay(_playerStats.Value.hitStunDuration, () =>
            {
                _onPlayerAddState.Call(PlayerState.None);
            }));
        }
        rseOnPlayerHealthReduced.Call(attackData.damage);
        _rseOnPlayerGettingHit.Call();
    }
}