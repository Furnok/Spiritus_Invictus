using System.Collections;
using UnityEngine;

public class S_PlayerHitResolver : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, S_AnimationName] string _hitParam;


    [Header("References")]
    [SerializeField] RSO_CanParry _canParry;
    [SerializeField] RSO_ParryStartTime _parryStartTime;
    [SerializeField] SSO_PlayerStats _playerStats;

    [Header("Input")]
    [SerializeField] RSE_OnAttackCollide _onAttackCollide;

    [Header("Output")]
    [SerializeField] RSE_OnParrySuccess _rseOnParrySuccess;
    [SerializeField] RSE_OnPlayerHit _rseOnPlayerHit;
    [SerializeField] private RSE_OnAnimationBoolValueChange rseOnAnimationBoolValueChange;

    private void OnEnable()
    {
        _onAttackCollide.action += ResolveHit;
    }

    private void OnDisable()
    {
        _onAttackCollide.action -= ResolveHit;
    }
    void ResolveHit(AttackData attackData)
    {
        if (attackData.attackType == EnemyAttackType.Parryable)
        {
            StartCoroutine(IsWithinParryWindowCoroutine((bool canParry) =>
            {
                if (canParry == true)
                {
                    _rseOnParrySuccess.Call(attackData);
                    Debug.Log("Parried!");
                }
                else
                {
                    Debug.Log("Hit!");
                    _rseOnPlayerHit.Call(attackData);
                    //rseOnAnimationBoolValueChange.Call("isHit", true);
                }
            }));
        }
        else if (attackData.attackType == EnemyAttackType.Parryable)
        {
            _rseOnPlayerHit.Call(attackData);
            Debug.Log("Player Hit P");
        }
        else if (attackData.attackType == EnemyAttackType.Dodgeable)
        {
            _rseOnPlayerHit.Call(attackData);
            Debug.Log("Player Hit d");
        }
       
    }
    IEnumerator IsWithinParryWindowCoroutine(System.Action<bool> callback)
    {
        float t = Time.time; //moment getting hit
        float start = _parryStartTime.Value; //when parry started
        float duration = _playerStats.Value.parryDuration;
        float tolBefore = _playerStats.Value.parryToleranceBeforeHit;
        float tolAfter = _playerStats.Value.parryToleranceAfterHit;

        if (t <= start + duration + tolBefore)
        {
            callback(true);
            yield break;
        }

        yield return new WaitForSeconds(tolAfter);

        start = _parryStartTime.Value;
        bool valid = t >= start - tolAfter && t <= start + duration + tolBefore;
        callback(valid);
    }
   
}