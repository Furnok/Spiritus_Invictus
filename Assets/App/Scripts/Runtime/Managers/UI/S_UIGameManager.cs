using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class S_UIGameManager : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Time")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float animationSlider;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeFadeBoss;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeFadeSkip;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float timeFadeConsole;

    [TabGroup("References")]
    [Title("Sliders")]
    [SerializeField] private Slider sliderBossHealth;

    [TabGroup("References")]
    [SerializeField] private Slider sliderHealth;

    [TabGroup("References")]
    [SerializeField] private Slider sliderConviction;

    [TabGroup("References")]
    [SerializeField] private Slider sliderPlayerAttackSteps;

    [TabGroup("References")]
    [Title("Skip")]
    [SerializeField] private GameObject skipWindow;

    [TabGroup("References")]
    [SerializeField] private Image imageSkip;

    [TabGroup("References")]
    [Title("Rect Transform Conviction")]
    [SerializeField] private RectTransform sliderFillAreaRectTransform;

    [TabGroup("References")]
    [SerializeField] private RectTransform ticksParent;

    [TabGroup("References")]
    [Title("Prefabs")]
    [SerializeField] private GameObject tickPrefab;

    [TabGroup("References")]
    [Title("Extract")]
    [SerializeField] private GameObject extractWindow;

    [TabGroup("References")]
    [Title("Console")]
    [SerializeField] private GameObject consoleWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnDisplayBossHealth rseOnDisplayBossHealth;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerHealthUpdate rseOnPlayerHealthUpdate;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerConvictionUpdate rseOnPlayerConvictionUpdate;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnDisplaySkip rseOnDisplaySkip;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnSkipHold rseOnSkipHold;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnOpenExtractWindow rseOnOpenExtractWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnUIInputEnabled rseOnUIInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnGameInputEnabled rseOnGameInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnDisplayExtract rseOnDisplayExtract;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnConsole rseOnConsole;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnOpenWindow rseOnOpenWindow;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PreconsumedConviction rsoPreconsumedConviction;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerStats ssoPlayerStats;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerConvictionData ssoPlayerConvictionData;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerAttackSteps ssoPlayerAttackSteps;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_Extract ssoExtractText;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_CameraData ssoCameraData;

    private Tween healthTween = null;
    private Tween convictionTween = null;
    private Tween preconvictionTween = null;
    private Tween skipTween = null;

    private bool isInConsole = false;

    private void Awake()
    {
        sliderHealth.maxValue = ssoPlayerStats.Value.maxHealth;
        sliderHealth.value = sliderHealth.maxValue;

        sliderConviction.maxValue = ssoPlayerConvictionData.Value.maxConviction;
        sliderConviction.value = sliderConviction.maxValue;

        sliderPlayerAttackSteps.maxValue = ssoPlayerConvictionData.Value.maxConviction;

        StartCoroutine(BuildTicksNextFrame());
    }

    private void OnEnable()
    {
        rseOnDisplayBossHealth.action += DisplayBossHealth;
        rseOnPlayerHealthUpdate.action += SetHealthSliderValue;
        rseOnPlayerConvictionUpdate.action += SetConvictionSliderValue;
        rsoPreconsumedConviction.onValueChanged += SetPreconvictionSliderValue;
        rseOnDisplaySkip.action += DisplaySkip;
        rseOnSkipHold.action += SetSkipHoldValue;
        rseOnOpenExtractWindow.action += DiplayExtract;
        rseOnConsole.action += Console;
    }

    private void OnDisable()
    {
        rseOnDisplayBossHealth.action -= DisplayBossHealth;
        rseOnPlayerHealthUpdate.action -= SetHealthSliderValue;
        rseOnPlayerConvictionUpdate.action -= SetConvictionSliderValue;
        rsoPreconsumedConviction.onValueChanged -= SetPreconvictionSliderValue;
        rseOnDisplaySkip.action -= DisplaySkip;
        rseOnSkipHold.action -= SetSkipHoldValue;
        rseOnOpenExtractWindow.action -= DiplayExtract;
        rseOnConsole.action -= Console;

        healthTween?.Kill();
        convictionTween?.Kill();
        preconvictionTween?.Kill();
    }

    private void DisplayBossHealth(bool value)
    {
        sliderBossHealth.GetComponent<CanvasGroup>()?.DOKill();

        if (value && !sliderBossHealth.gameObject.activeInHierarchy)
        {
            sliderBossHealth.gameObject.gameObject.SetActive(true);

            sliderBossHealth.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
            sliderBossHealth.gameObject.GetComponent<CanvasGroup>().DOFade(1f, timeFadeBoss).SetEase(Ease.Linear);
        }
        else if (!value)
        {
            sliderBossHealth.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            sliderBossHealth.gameObject.GetComponent<CanvasGroup>().DOFade(0f, timeFadeBoss).SetEase(Ease.Linear).OnComplete(() =>
            {
                sliderBossHealth.gameObject.SetActive(false);
            });
        }
    }

    private void SetHealthSliderValue(float health)
    {
        healthTween?.Kill();

        healthTween = sliderHealth.DOValue(health, animationSlider).SetEase(Ease.OutCubic);
    }

    private void SetConvictionSliderValue(float conviction)
    {
        convictionTween?.Kill();

        convictionTween = sliderConviction.DOValue(conviction, animationSlider).SetEase(Ease.OutCubic);
    }

    private void SetPreconvictionSliderValue(float preconvition)
    {
        preconvictionTween?.Kill();

        preconvictionTween = sliderPlayerAttackSteps.DOValue(preconvition, animationSlider).SetEase(Ease.OutCubic);
    }

    private IEnumerator BuildTicksNextFrame()
    {
        yield return null;

        CreateStepTicks();
    }

    private void CreateStepTicks()
    {
        for (int i = ticksParent.childCount - 1; i >= 0; i--)
        {
            Destroy(ticksParent.GetChild(i).gameObject);
        }

        float maxConv = ssoPlayerConvictionData.Value.maxConviction;

        var fillRect = sliderFillAreaRectTransform;

        if (ticksParent != fillRect)
        {
            ticksParent.SetParent(fillRect, worldPositionStays: false);
            ticksParent.anchorMin = new Vector2(0, 0.5f);
            ticksParent.anchorMax = new Vector2(1, 0.5f);
            ticksParent.pivot = new Vector2(0.5f, 0.5f);
            ticksParent.offsetMin = Vector2.zero;
            ticksParent.offsetMax = Vector2.zero;
            ticksParent.anchoredPosition = Vector2.zero;
        }

        foreach (var step in ssoPlayerAttackSteps.Value)
        {
            float normalized = Mathf.Clamp01(step.ammountConvitionNeeded / maxConv);

            var tickGO = Instantiate(tickPrefab, ticksParent);
            var rt = (RectTransform)tickGO.transform;

            rt.anchorMin = new Vector2(normalized, 0.5f);
            rt.anchorMax = new Vector2(normalized, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }
    }

    private void DisplaySkip(bool value)
    {
        skipWindow.GetComponent<CanvasGroup>()?.DOKill();

        if (value && !skipWindow.activeInHierarchy)
        {
            skipWindow.SetActive(true);

            skipWindow.GetComponent<CanvasGroup>().alpha = 0f;
            skipWindow.GetComponent<CanvasGroup>().DOFade(1f, timeFadeSkip).SetEase(Ease.Linear);
        }
        else if (!value)
        {
            skipWindow.SetActive(false);
        }
    }

    private void SetSkipHoldValue(float value)
    {
        skipTween?.Kill();

        skipTween = imageSkip.DOFillAmount(value / ssoCameraData.Value.holdSkipTime, animationSlider).SetEase(Ease.OutCubic);
    }

    private void DiplayExtract(int index)
    {
        rseOnUIInputEnabled.Call();
        rseOnOpenWindow.Call(extractWindow);
        rseOnDisplayExtract.Call(ssoExtractText.Value[index]);
    }

    private void Console()
    {
        if (!isInConsole)
        {
            isInConsole = true;

            rseOnUIInputEnabled.Call();

            if (!consoleWindow.activeInHierarchy)
            {
                consoleWindow.GetComponent<CanvasGroup>()?.DOKill();

                consoleWindow.SetActive(true);

                consoleWindow.GetComponent<CanvasGroup>().alpha = 0f;
                consoleWindow.GetComponent<CanvasGroup>().DOFade(1f, timeFadeConsole).SetEase(Ease.Linear).SetUpdate(true);
            }
        }
        else
        {
            isInConsole = false;

            rseOnGameInputEnabled.Call();
        }
    }
}