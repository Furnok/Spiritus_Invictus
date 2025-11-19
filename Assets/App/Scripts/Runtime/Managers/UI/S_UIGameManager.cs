using DG.Tweening;
using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [Title("Audio")]
    [SerializeField] private EventReference uiSound;

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

    [TabGroup("References")]
    [SerializeField] private GameObject consoleBackgroundWindow;

    [TabGroup("References")]
    [SerializeField] private Selectable buttonSend;

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
    [SerializeField] private RSE_OnShowMouseCursor rseOnShowMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnHideMouseCursor rseOnHideMouseCursor;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnResetFocus rseOnResetFocus;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_PreconsumedConviction rsoPreconsumedConviction;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_ConsoleDisplay rsoConsoleDisplay;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_InConsole rsoInConsole;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_GameInPause rsoGameInPause;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_Navigation rsoNavigation;

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

    private void Awake()
    {
        rsoConsoleDisplay.Value = new();
        rsoInConsole.Value = new();

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
        RuntimeManager.PlayOneShot(uiSound);

        rseOnUIInputEnabled.Call();
        rseOnOpenWindow.Call(extractWindow);
        rseOnDisplayExtract.Call(ssoExtractText.Value[index]);
    }

    private void Console()
    {
        if (!rsoConsoleDisplay.Value)
        {
            rsoNavigation.Value.selectablePressOld = rsoNavigation.Value.selectableFocus;
            rsoNavigation.Value.selectableDefault = null;
            rseOnResetFocus.Call();
            rsoNavigation.Value.selectableFocus = null;

            if (Gamepad.current == null)
            {
                rseOnShowMouseCursor.Call();
            }

            RuntimeManager.PlayOneShot(uiSound);

            rsoConsoleDisplay.Value = true;
            rsoInConsole.Value = true;

            rseOnUIInputEnabled.Call();

            consoleWindow.GetComponent<CanvasGroup>()?.DOKill();

            consoleWindow.SetActive(true);

            consoleWindow.GetComponent<CanvasGroup>().alpha = 0f;
            consoleWindow.GetComponent<CanvasGroup>().DOFade(1f, timeFadeConsole).SetEase(Ease.Linear).SetUpdate(true);
        }
        else
        {
            if (!rsoInConsole.Value)
            {
                rsoNavigation.Value.selectablePressOld = rsoNavigation.Value.selectableFocus;
                rsoNavigation.Value.selectableDefault = null;
                rseOnResetFocus.Call();
                rsoNavigation.Value.selectableFocus = null;

                rsoNavigation.Value.selectableDefault = buttonSend;
                buttonSend?.Select();

                if (Gamepad.current == null)
                {
                    rseOnShowMouseCursor.Call();
                }

                rsoInConsole.Value = true;

                rseOnUIInputEnabled.Call();

                consoleBackgroundWindow.GetComponent<CanvasGroup>().alpha = 0f;
                consoleBackgroundWindow.GetComponent<CanvasGroup>().DOFade(1f, timeFadeConsole).SetEase(Ease.Linear).SetUpdate(true);
                consoleBackgroundWindow.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                if (rsoNavigation.Value.selectablePressOld != null)
                {
                    rseOnResetFocus.Call();
                    rsoNavigation.Value.selectableDefault = rsoNavigation.Value.selectablePressOld;
                    rsoNavigation.Value.selectableFocus = rsoNavigation.Value.selectablePressOld;
                    rsoNavigation.Value.selectablePressOld = null;
                    rsoNavigation.Value.selectableDefault.Select();
                }
                else
                {
                    rsoNavigation.Value.selectableDefault = null;
                    rseOnResetFocus.Call();
                    rsoNavigation.Value.selectableFocus = null;
                }

                RuntimeManager.PlayOneShot(uiSound);

                consoleWindow.GetComponent<CanvasGroup>()?.DOKill();

                consoleWindow.GetComponent<CanvasGroup>().alpha = 1f;
                consoleWindow.GetComponent<CanvasGroup>().DOFade(0f, timeFadeConsole).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
                {
                    rseOnHideMouseCursor.Call();

                    rsoConsoleDisplay.Value = false;
                    rsoInConsole.Value = false;

                    if (rsoGameInPause.Value)
                    {
                        rseOnUIInputEnabled.Call();

                        rseOnShowMouseCursor.Call();
                    }
                    else
                    {
                        rseOnGameInputEnabled.Call();
                    }

                    consoleWindow.SetActive(false);
                });
            }
        }
    }
}