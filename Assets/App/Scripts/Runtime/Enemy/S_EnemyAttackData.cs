using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class S_EnemyAttackData : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Time")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeDisplay;

    [TabGroup("References")]
    [Title("Collider")]
    [SerializeField] private Collider weaponCollider;

    [TabGroup("References")]
    [Title("Image")]
    [SerializeField] private Image warning;

    [HideInInspector] public UnityEvent<S_StructEnemyAttackData> onChangeAttackData;

    private S_StructEnemyAttackData attackData;

    public void SetAttackMode(S_StructEnemyAttackData enemyAttackData)
    {
        attackData = enemyAttackData;
        onChangeAttackData.Invoke(enemyAttackData);
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

    public void DisplayTriggerWarning()
    {
        if (warning != null)
        {
            if (attackData.attackType == S_EnumEnemyAttackType.Parryable || attackData.attackType == S_EnumEnemyAttackType.Projectile)
            {
                warning.color = Color.yellow;
            }
            else if (attackData.attackType == S_EnumEnemyAttackType.Dodgeable)
            {
                warning.color = Color.red;
            }

            StartCoroutine(DisplayWarning());
        }
    }

    private IEnumerator DisplayWarning()
    {
        warning.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeDisplay);
        warning.gameObject.SetActive(false);
    }
}