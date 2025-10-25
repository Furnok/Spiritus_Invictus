using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TestConvictionBarUi : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] private SSO_PlayerConvictionData _playerConvictionData;
    [SerializeField] private RSO_PlayerCurrentConviction _playerCurrentConviction;
    [SerializeField] RSO_PreconsumedConviction _preconsumedConviction;
    [SerializeField] private SSO_PlayerAttackSteps _playerAttackSteps;
    [SerializeField] Slider _sliderConviction;
    [SerializeField] Slider _sliderAttackStepProgression;
    [SerializeField] RectTransform _sliderFillAreaRectTransform;
    [SerializeField] private RectTransform _ticksParent;
    [SerializeField] private GameObject _tickPrefab;

    //[Header("Input")]

    //[Header("Output")]

    private void Awake()
    {
        _sliderConviction.maxValue = _playerConvictionData.Value.maxConviction;

        _sliderAttackStepProgression.maxValue = _playerConvictionData.Value.maxConviction;

        StartCoroutine(BuildTicksNextFrame());
    }

    private void OnEnable()
    {
        _playerCurrentConviction.onValueChanged += SetConvictionSliderValue;
        _preconsumedConviction.onValueChanged += SetPreconvictionSliderValue;
    }

    private void OnDisable()
    {
        _playerCurrentConviction.onValueChanged -= SetConvictionSliderValue;
        _preconsumedConviction.onValueChanged -= SetPreconvictionSliderValue;
    }

    void SetConvictionSliderValue(float conviction)
    {
        _sliderConviction.value = conviction;
    }

    void SetPreconvictionSliderValue(float preconvition)
    {
        _sliderAttackStepProgression.value = preconvition;
    }

    private IEnumerator BuildTicksNextFrame()
    {
        yield return null;
        CreateStepTicks();
    }

    void CreateStepTicks()
    {
        for (int i = _ticksParent.childCount - 1; i >= 0; i--)
            Destroy(_ticksParent.GetChild(i).gameObject);

        float maxConv = _playerConvictionData.Value.maxConviction;

        var fillRect = _sliderFillAreaRectTransform;
        if (_ticksParent != fillRect)
        {
            _ticksParent.SetParent(fillRect, worldPositionStays: false);
            _ticksParent.anchorMin = new Vector2(0, 0.5f);
            _ticksParent.anchorMax = new Vector2(1, 0.5f);
            _ticksParent.pivot = new Vector2(0.5f, 0.5f);
            _ticksParent.offsetMin = Vector2.zero;
            _ticksParent.offsetMax = Vector2.zero;
            _ticksParent.anchoredPosition = Vector2.zero;
        }

        foreach (var step in _playerAttackSteps.Value)
        {
            //if (step.ammountConvitionNeeded <= 0f || step.ammountConvitionNeeded > maxConv) //Eventually unallow 0 and max conviction steps
            //    continue;

            float normalized = Mathf.Clamp01(step.ammountConvitionNeeded / maxConv);

            var tickGO = Instantiate(_tickPrefab, _ticksParent);
            var rt = (RectTransform)tickGO.transform;

            rt.anchorMin = new Vector2(normalized, 0.5f);
            rt.anchorMax = new Vector2(normalized, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }
    }
    
}