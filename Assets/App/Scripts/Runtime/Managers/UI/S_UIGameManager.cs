using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class S_UIGameManager : MonoBehaviour
{
    [TabGroup("References")]
    [Title("Sliders")]
    [SerializeField] private Slider sliderBossHealth;

    [TabGroup("References")]
    [SerializeField] private Slider sliderHealth;

    [TabGroup("References")]
    [SerializeField] private Slider sliderConviction;

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
    [SerializeField] private GameObject extractCanvas;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnOpenExtractWindow rseOnOpenExtractWindow;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnDisplayBossHealth rseOnDisplayBossHealth;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerHealthUpdate rseOnPlayerHealthUpdate;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerConvictionUpdate rseOnPlayerConvictionUpdate;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnUIInputEnabled rseOnUIInputEnabled;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnDisplayExtract rseOnDisplayExtract;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnOpenWindow rseOnOpenWindow;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerStats ssoPlayerStats;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerConvictionData ssoPlayerConvictionData;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerAttackSteps ssoPlayerAttackSteps;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_ExtractText ssoExtractText;

    private void Awake()
    {
        sliderHealth.maxValue = ssoPlayerStats.Value.maxHealth;
        sliderHealth.value = sliderHealth.maxValue;

        sliderConviction.maxValue = ssoPlayerConvictionData.Value.maxConviction;
        sliderConviction.value = sliderConviction.maxValue;

        StartCoroutine(BuildTicksNextFrame());
    }

    private void OnEnable()
    {
        rseOnDisplayBossHealth.action += DisplayBossHealth;
        rseOnPlayerHealthUpdate.action += SetHealthSliderValue;
        rseOnPlayerConvictionUpdate.action += SetConvictionSliderValue;
        rseOnOpenExtractWindow.action += DiplayExtract;
    }

    private void OnDisable()
    {
        rseOnDisplayBossHealth.action -= DisplayBossHealth;
        rseOnPlayerHealthUpdate.action -= SetHealthSliderValue;
        rseOnPlayerConvictionUpdate.action -= SetConvictionSliderValue;
        rseOnOpenExtractWindow.action -= DiplayExtract;
    }

    private void DisplayBossHealth(bool value)
    {
        sliderBossHealth.gameObject.SetActive(value);
    }

    private void SetHealthSliderValue(float health)
    {
        sliderHealth.value = health;
    }

    void SetConvictionSliderValue(float conviction)
    {
        sliderConviction.value = conviction;
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

    private void DiplayExtract(int index)
    {
        rseOnUIInputEnabled.Call();
        rseOnOpenWindow.Call(extractCanvas);
        rseOnDisplayExtract.Call(ssoExtractText.Value[index]);
    }
}