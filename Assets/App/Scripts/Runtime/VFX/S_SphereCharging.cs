using System.Linq;
using UnityEngine;

public class S_SphereCharging : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _minScale = 0.2f;
    [SerializeField] private float _maxScale = 1f;
    [SerializeField] private float _pulseSpeed = 15f;
    [SerializeField] private float _pulseAmplitude = 0.1f;

    [Header("References")]
    [SerializeField] private Transform _energySphere;
    [SerializeField] private SSO_PlayerAttackSteps _playerAttackSteps;
    [SerializeField] private SSO_PlayerStats _playerStats;

    //[Header("Inputs")]

    //[Header("Outputs")]

    private float charge = 0f; // 0  1
    private bool isCharging = true;
    private float _chargeDuration => GetMaxHoldTime();


    void Update()
    {
        if (isCharging)
            charge = Mathf.Clamp01(charge + Time.deltaTime / _chargeDuration);

        float baseScale = Mathf.Lerp(_minScale, _maxScale, charge);
        float pulse = 1f + Mathf.Sin(Time.time * _pulseSpeed) * _pulseAmplitude * charge;
        float finalScale = baseScale * pulse;

        _energySphere.localScale = Vector3.one * finalScale;
    }

    void OnEnable()
    {
        charge = 0f;
        isCharging = true;
    }

    void OnDisable()
    {
        isCharging = false;
        _energySphere.localScale = _minScale * Vector3.one;
    }

    private float GetMaxHoldTime()
    {
        float max = 0f;

        foreach (var step in _playerAttackSteps.Value)
        {
            if (step.timeHoldingInput > max)
                max = step.timeHoldingInput;
        }

        max += _playerStats.Value.timeWaitBetweenSteps * _playerAttackSteps.Value.Count - 1;

        return max;
    }
}