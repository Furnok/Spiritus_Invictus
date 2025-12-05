using DG.Tweening;
using Sirenix.OdinInspector;
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
    [SerializeField] private GameObject content;

    [TabGroup("References")]
    [Title("Image")]
    [SerializeField] private Image warning;

    [TabGroup("References")]
    [Title("Script")]
    [SerializeField] private S_Enemy enemy;

    [HideInInspector] public UnityEvent<S_StructEnemyAttackData> onChangeAttackData = null;

    private S_StructEnemyAttackData attackData;

    private Tween fadeTween = null;

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
            fadeTween?.Kill();

            if (attackData.attackType == S_EnumEnemyAttackType.Parryable || attackData.attackType == S_EnumEnemyAttackType.Projectile)
            {
                warning.color = Color.yellow;
            }
            else if (attackData.attackType == S_EnumEnemyAttackType.Dodgeable)
            {
                warning.color = Color.red;
            }

            content.gameObject.gameObject.SetActive(true);

            fadeTween = content.gameObject.GetComponent<CanvasGroup>().DOFade(1f, timeDisplay).SetEase(Ease.Linear);
        }
    }

    public void UnDisplayTriggerWarning()
    {
        fadeTween?.Kill();

        fadeTween = content.gameObject.GetComponent<CanvasGroup>().DOFade(0f, timeDisplay).SetEase(Ease.Linear).OnComplete(() =>
        {
            content.gameObject.SetActive(false);
        });
    }

    public void Rotate()
    {
        enemy.RotateEnemy();
    }
}