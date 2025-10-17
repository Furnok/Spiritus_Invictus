using UnityEngine;
using UnityEngine.Events;

public class S_EnemyAttackData : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] Collider weaponCollider;

    //[Header("Input")]

    //[Header("Output")]
    [HideInInspector] public UnityEvent<SSO_EnemyAttackData> onChangeAttackData;


    public void SetAttackMode(SSO_EnemyAttackData SSO_AttackData)
    {
        onChangeAttackData.Invoke(SSO_AttackData);
        Debug.Log("GetData");
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