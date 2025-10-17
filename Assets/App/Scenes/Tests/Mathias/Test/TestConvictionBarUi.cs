using UnityEngine;
using UnityEngine.UI;

public class TestConvictionBarUi : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] private SSO_PlayerConvictionData _playerConvictionData;
    [SerializeField] private RSO_PlayerCurrentConviction _playerCurrentConviction;
    [SerializeField] Slider _sliderConviction;

    //[Header("Input")]

    //[Header("Output")]

    private void Awake()
    {
        _sliderConviction.maxValue = _playerConvictionData.Value.maxConviction;
        _sliderConviction.value = _sliderConviction.maxValue;
    }

    private void OnEnable()
    {
        _playerCurrentConviction.onValueChanged += SetHealthSliderValue;
    }

    private void OnDisable()
    {
        _playerCurrentConviction.onValueChanged -= SetHealthSliderValue;
    }

    void SetHealthSliderValue(float health)
    {
        _sliderConviction.value = health;
    }
}