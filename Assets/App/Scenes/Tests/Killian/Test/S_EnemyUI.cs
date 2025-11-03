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
    [SerializeField] private float timeDisplayHealthBar;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float animationSlider;

    [TabGroup("References")]
    [Title("Content")]
    [SerializeField] private GameObject content;

    [TabGroup("References")]
    [SerializeField] private Slider sliderHealth;

    [TabGroup("References")]
    [Title("Script")]
    [SerializeField] private S_Enemy enemy;

    private Coroutine displayHealthBar;
    private Tween healthTween;

    private void OnEnable()
    {
        enemy.onUpdateEnemyHealth.AddListener(UpdateHealthBar);
    }

    private void OnDisable()
    {
        enemy.onUpdateEnemyHealth.RemoveListener(UpdateHealthBar);

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
        content.SetActive(false);
    }

    private void UpdateHealthBar(float healthValue)
    {
        if (healthValue <= 0)
        {
            content.SetActive(false);
        }
        else
        {
            content.SetActive(true);
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
        content.SetActive(false);
        displayHealthBar = null;
    }
}