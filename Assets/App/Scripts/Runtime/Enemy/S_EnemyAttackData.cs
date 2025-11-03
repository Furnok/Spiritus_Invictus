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
        S_StructEnemyAttackData temp = ssoAttackData.Value;

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

        onChangeAttackData.Invoke(temp);
    }

    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
    }

    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }
}