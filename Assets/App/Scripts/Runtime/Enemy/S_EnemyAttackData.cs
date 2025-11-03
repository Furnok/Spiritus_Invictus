using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class S_EnemyAttackData : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] Collider weaponCollider;

    [HideInInspector] public UnityEvent<S_StructEnemyAttackData> onChangeAttackData;

    private bool setup = false;
    private float damageDodge = 0;
    private float damageParry = 0;

    public void Setup(SSO_EnemyData ssoEnemyData)
    {
        damageDodge = ssoEnemyData.Value.attackLightDamage;
        damageParry = ssoEnemyData.Value.attackHeavyDamage;
        setup = true;
    }

    public void SetAttackMode(SSO_EnemyAttackData ssoAttackData)
    {
        S_StructEnemyAttackData temp = new();

        temp.attackId = ssoAttackData.Value.attackId;
        temp.attackType = ssoAttackData.Value.attackType;

        if (setup)
        {
            if (temp.attackType == S_EnumEnemyAttackType.Dodgeable)
            {
                temp.damage = damageDodge;
            }
            else if (temp.attackType == S_EnumEnemyAttackType.Parryable)
            {
                temp.damage = damageParry;
            }
        }
        else
        {
           temp.damage = ssoAttackData.Value.damage;
        }

        temp.convictionReduction = ssoAttackData.Value.convictionReduction;

        onChangeAttackData.Invoke(temp);
    }

    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
    }

    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }
}