using UnityEngine;
using UnityEngine.UI;

public class TestHealthBarUi : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] private SSO_PlayerStats _playerStats;
    [SerializeField] private RSO_PlayerCurrentHealth _playerCurrentHealth;
    [SerializeField] Slider _sliderHealth;

    //[Header("Input")]

    //[Header("Output")]

    private void Awake()
    {
        _sliderHealth.maxValue = _playerStats.Value.maxHealth;
        _sliderHealth.value = _sliderHealth.maxValue;
    }

    private void OnEnable()
    {
        _playerCurrentHealth.onValueChanged += SetHealthSliderValue;
    }

    private void OnDisable()
    {
        _playerCurrentHealth.onValueChanged -= SetHealthSliderValue;
    }

    void SetHealthSliderValue(float health)
    {
        _sliderHealth.value = health;
    }
}