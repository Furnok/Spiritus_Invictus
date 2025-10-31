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
    [SerializeField] RSO_AttackDataInDodgeableArea _attackDataInDodgeableArea;
    [SerializeField] RSO_AttackCanHitPlayer _attackCanHitPlayer;
    [SerializeField] SSO_PlayerConvictionData _playerConvictionData;

    [Header("Input")]
    [SerializeField] RSE_OnAttackCollide _onAttackCollide;

    [Header("Output")]
    [SerializeField] RSE_OnParrySuccess _rseOnParrySuccess;
    [SerializeField] RSE_OnPlayerHit _rseOnPlayerHit;
    [SerializeField] RSE_OnPlayerGainConviction _onPlayerGainConviction;


    private void OnEnable()
    {
        _onAttackCollide.action += ResolveHit;
    }

    private void OnDisable()
    {
        _onAttackCollide.action -= ResolveHit;
    }
    void ResolveHit(AttackContact contact)
    {
        var attackData = contact.data;

        if (attackData.attackType == EnemyAttackType.Parryable)
        {

            StartCoroutine(IsWithinParryWindowCoroutine((bool canParry) =>
            {
                if (canParry == true)
                {
                    _rseOnParrySuccess.Call(attackData);
                    _onPlayerGainConviction.Call(_playerConvictionData.Value.parrySuccesGain);
                    Debug.Log("Parried!");
                }
                else
                {
                    _rseOnPlayerHit.Call(attackData);
                }
            }));
        }
        else if (attackData.attackType == EnemyAttackType.Dodgeable)
        {
            var canHit = _attackCanHitPlayer.Value.ContainsKey(attackData.goSourceId);

            if (canHit == true)
            {
                _rseOnPlayerHit.Call(attackData);
            }
            else
            {
                Debug.Log("Dont hit cause dodge perfect");
            }


        }
        else if (attackData.attackType == EnemyAttackType.Projectile)
        {
            
            StartCoroutine(IsWithinParryWindowCoroutine((bool canParry) =>
            {
                if (canParry == true)
                {
                    _rseOnParrySuccess.Call(attackData);
                    _onPlayerGainConviction.Call(_playerConvictionData.Value.parrySuccesGain);

                    TryReflectProjectile(contact.source);
                    Debug.Log("Parried!");
                }
                else
                {
                    Destroy(contact.source.gameObject);
                    _rseOnPlayerHit.Call(attackData);
                }
            }));

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

    void TryReflectProjectile(Collider source)
    {
        if (source == null) return;

        if (source.TryGetComponent<IReflectableProjectile>(out var proj))
        {
            proj.Reflect();
        }
        else
        {
            var p = source.GetComponentInParent<IReflectableProjectile>();
            if (p != null) p.Reflect();
        }
    }

}