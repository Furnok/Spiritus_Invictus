using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class S_EnemyUI : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Time")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeFade;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeDisplayHealthBar;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float animationSlider;

    [TabGroup("References")]
    [Title("Content")]
    [SerializeField] private GameObject content;

    [TabGroup("References")]
    [SerializeField] private Slider sliderHealth;

    private Coroutine displayHealthBar;
    private Tween healthTween;

    private void OnDisable()
    {
        healthTween?.Kill();
    }

    private void Update()
    {
        sliderHealth.gameObject.transform.LookAt(Camera.main.transform);
    }

    public void Setup(SSO_EnemyData ssoEnemyData)
    {
        sliderHealth.maxValue = ssoEnemyData.Value.health;
        sliderHealth.value = ssoEnemyData.Value.health;

        if (content.activeInHierarchy)
        {
            CanvasGroup cg = content.GetComponent<CanvasGroup>();
            cg.DOKill();

            cg.DOFade(0f, timeFade).SetEase(Ease.Linear).OnComplete(() =>
            {
                content.SetActive(false);
            });
        }
    }

    public void UpdateHealthBar(float healthValue)
    {
        CanvasGroup cg = content.GetComponent<CanvasGroup>();
        cg.DOKill();

        if (healthValue <= 0)
        {
            cg.DOFade(0f, timeFade).SetEase(Ease.Linear).OnComplete(() =>
            {
                content.SetActive(false);
            });
        }
        else
        {
            content.gameObject.SetActive(true);

            cg.DOFade(1f, timeFade).SetEase(Ease.Linear);
        }

        healthTween?.Kill();

        healthTween = sliderHealth.DOValue(healthValue, animationSlider).SetEase(Ease.OutCubic);

        if (displayHealthBar != null)
        {
            StopCoroutine(displayHealthBar);
            displayHealthBar = null;
        }

        displayHealthBar = StartCoroutine(DisplayHealthBar());
    }

    private IEnumerator DisplayHealthBar()
    {
        yield return new WaitForSeconds(timeDisplayHealthBar);

        CanvasGroup cg = content.GetComponent<CanvasGroup>();
        cg.DOKill();

        cg.DOFade(0f, timeFade).SetEase(Ease.Linear).OnComplete(() =>
        {
            content.SetActive(false);
        });
    }
}