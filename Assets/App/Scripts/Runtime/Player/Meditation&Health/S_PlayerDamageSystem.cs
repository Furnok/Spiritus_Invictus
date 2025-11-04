using UnityEngine;

public class S_PlayerDamageSystem : MonoBehaviour
{
    [Header("Reference")]

    [SerializeField] SSO_PlayerStateTransitions _playerStateTransitions;
    [SerializeField] RSO_PlayerCurrentState _playerCurrentState;
    [SerializeField] SSO_PlayerStats _playerStats;
    [SerializeField] RSO_IsInvicible _isInvicible;
    [SerializeField] SSO_DebugPlayer _debugPlayer;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerTakeDamage rseOnPlayerTakeDamage;
    [SerializeField] RSE_OnPlayerHit _rseOnPlayerHit;

    [Header("Output")]
    [SerializeField] private RSE_OnPlayerHealthReduced rseOnPlayerHealthReduced;
    [SerializeField] RSE_OnAnimationTriggerValueChange rseOnAnimationTriggerValueChange;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;
    [SerializeField] RSE_OnPlayerAddState _onPlayerAddState;

    Coroutine _hitReactCoroutine;
    private void OnEnable()
    {
        _isInvicible.Value = false;
        rseOnPlayerTakeDamage.action += TakeDamage;
        _rseOnPlayerHit.action += TakeDamage;
    }
    
    private void OnDisable()
    {
        rseOnPlayerTakeDamage.action -= TakeDamage;
        _rseOnPlayerHit.action -= TakeDamage;
        _isInvicible.Value = false;

    }

    private void TakeDamage(float damage)
    {
        //rseOnAnimationTriggerValueChange.Call("isHit");
        //rseOnPlayerHealthReduced.Call(damage);
        //_rseOnPlayerGettingHit.Call();
    }


    private void TakeDamage(S_StructEnemyAttackData attackData)
    {

        if (_playerStateTransitions.CanTransition(_playerCurrentState.Value, PlayerState.HitReact) == true && _isInvicible.Value == false && _debugPlayer.Value.cantGetttingHit == false)
        {
            if (_hitReactCoroutine != null)
            {
                StopCoroutine(_hitReactCoroutine);
            }

            rseOnAnimationTriggerValueChange.Call("isHit");
            _onPlayerAddState.Call(PlayerState.HitReact);
            _isInvicible.Value = true;

            _hitReactCoroutine = StartCoroutine(S_Utils.Delay(attackData.knockbackDuration, () =>
            {
                _onPlayerAddState.Call(PlayerState.None);
            }));

            StartCoroutine(S_Utils.Delay(attackData.invicibilityDuration, () =>
            {
                _isInvicible.Value = false;
            }));

            rseOnPlayerHealthReduced.Call(attackData.damage);
            _rseOnPlayerGettingHit.Call();
        }
        
    }
}