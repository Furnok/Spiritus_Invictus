using DG.Tweening;
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
    [SerializeField] private GameObject content;

    [TabGroup("References")]
    [Title("Image")]
    [SerializeField] private Image warning;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnGamePause rseOnGamePause;

    [HideInInspector] public UnityEvent<S_StructEnemyAttackData> onChangeAttackData;

    private S_StructEnemyAttackData attackData;
    private bool isPaused = false;

    private void OnEnable()
    {
        rseOnGamePause.action += Pause;
    }

    private void OnDisable()
    {
        rseOnGamePause.action -= Pause;
    }

    private void Pause(bool value)
    {
        if (value)
        {
            isPaused = true;
        }
        else
        {
            isPaused = false;
        }
    }

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
            content.gameObject.GetComponent<CanvasGroup>()?.DOKill();

            if (attackData.attackType == S_EnumEnemyAttackType.Parryable || attackData.attackType == S_EnumEnemyAttackType.Projectile)
            {
                warning.color = Color.yellow;
            }
            else if (attackData.attackType == S_EnumEnemyAttackType.Dodgeable)
            {
                warning.color = Color.red;
            }

            content.gameObject.gameObject.SetActive(true);

            content.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
            content.gameObject.GetComponent<CanvasGroup>().DOFade(1f, timeDisplay).SetEase(Ease.Linear);
        }
    }

    public void UnDisplayTriggerWarning()
    {
        content.gameObject.GetComponent<CanvasGroup>()?.DOKill();

        content.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        content.gameObject.GetComponent<CanvasGroup>().DOFade(0f, timeDisplay).SetEase(Ease.Linear).OnComplete(() =>
        {
            content.gameObject.SetActive(false);
        });
    }
}