using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class S_EnemyAttackData : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] Collider weaponCollider;

    [HideInInspector] public UnityEvent<SSO_EnemyAttackData> onChangeAttackData;

    public void SetAttackMode(SSO_EnemyAttackData SSO_AttackData)
    {
        onChangeAttackData.Invoke(SSO_AttackData);
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