using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class S_BossAttackData : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Times")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeDisplay;

    [TabGroup("References")]
    [Title("Colliders")]
    [SerializeField] private Collider weaponCollider;

    [TabGroup("References")]
    [Title("Scripts")]
    [SerializeField] private S_Boss boss;

    [TabGroup("References")]
    [SerializeField] private S_EnemyWeapon enemyWeapon;

    private S_StructEnemyAttackData attackData;

    public void SetAttackMode(S_StructEnemyAttackData bossAttackData)
    {
        attackData = bossAttackData;

        if (enemyWeapon != null) enemyWeapon.ChangeAttackData(attackData);
    }

    public void EnableWeaponCollider()
    {
        if (weaponCollider != null) weaponCollider.enabled = true;
    }

    public void DisableWeaponCollider()
    {
        if (weaponCollider != null) weaponCollider.enabled = false;
    }

    public void Rotate()
    {
        boss.RotateEnemyAnim();
    }

    public void StopRotate()
    {
        boss.StopRotateEnemyAnim();
    }

    public void PlayFmod(string eventName)
    {
        RuntimeManager.PlayOneShot(eventName, transform.position);
    }
}