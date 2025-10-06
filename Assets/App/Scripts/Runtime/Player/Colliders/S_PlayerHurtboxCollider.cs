using UnityEditor.Build.Pipeline;
using UnityEngine;

public class S_PlayerHurtboxCollider : MonoBehaviour
{
    //[Header("Settings")]
    

    [Header("References")]
    [SerializeField] RSO_CanParry _canParry;
    [SerializeField] RSO_ParryStartTime _parryStartTime;
    [SerializeField] SSO_PlayerStats _playerStats;


    //[Header("Input")]

    //[Header("Output")]

    float _parryToleranceBeforeHit => _playerStats.Value.parryToleranceBeforeHit;
    float _parryToleranceAfterHit => _playerStats.Value.parryToleranceAfterHit;
    float _parryDuration => _playerStats.Value.parryDuration;

    float _hitTime;

    private void OnTriggerEnter(Collider other)
    {
        if(other != null && other.CompareTag("Hitbox"))
        {
            IAttackProvider attackProvider = other.GetComponent<IAttackProvider>();

            if(attackProvider != null)
            {
                EnemyAttackType attackType = attackProvider.GetAttackType();

                if(attackType == EnemyAttackType.Parryable)
                {
                    _hitTime = Time.time;
                }

                if (attackType == EnemyAttackType.Parryable && _canParry.Value == true)
                {
                    //Parry
                    Debug.Log("Parry 1");
                }
                else if(attackType == EnemyAttackType.Parryable && _canParry.Value == false)
                {
                    if (CanParryAtTime(_hitTime))
                    {
                        //Parry
                        Debug.Log("Parry 2");

                    }
                    else
                    {
                        StartCoroutine(S_Utils.Delay(_parryToleranceAfterHit, () =>
                        {
                            if (_canParry.Value == true)
                            {
                                //Parry
                                Debug.Log("Parry 3");

                            }
                            else
                            {
                                Debug.Log("get Hit");
                            }
                        }));
                    }
                }
                else if (attackType == EnemyAttackType.Dodgeable)
                {

                }
                _parryStartTime.Value = 0f;
            }
        }
    }

    bool CanParryAtTime(float hitTime)
    {
        return Mathf.Abs(hitTime - _parryStartTime.Value) <= _parryDuration + _parryToleranceBeforeHit;
    }
}

public interface IAttackProvider
{
    //public EnemyAttackType _attackType { get; set; }
    EnemyAttackType GetAttackType();
}