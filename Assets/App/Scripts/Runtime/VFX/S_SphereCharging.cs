using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_SphereCharging : MonoBehaviour
{
    [Header("Sphere Settings")]
    [SerializeField] private float _minScale = 0.2f;
    [SerializeField] private float _maxScale = 1f;
    [SerializeField] private float _pulseSpeed = 15f;
    [SerializeField] private float _pulseAmplitude = 0.1f;

    [Header("Particle parameters")]
    [SerializeField] private List<ParticleSettingsData> _listParticleSettingsData = new List<ParticleSettingsData>();
    


    [Header("References")]
    [SerializeField] private Transform _energySphere;
    [SerializeField] private SSO_PlayerAttackSteps _playerAttackSteps;
    [SerializeField] private SSO_PlayerStats _playerStats;
    [SerializeField] private ParticleSystem _particleSfxChargeAttack;
    [SerializeField] private Material _materialEnergySphere;
    [SerializeField] private RSO_CurrentChargeStep _rsoCurrentChargeStep;

    [System.Serializable]
    public struct ParticleSettingsData
    {
        public int Step;
        public float SpeedModifier;
        public float ParticlesEmission;
        public float ParticlesOrbitalMinEmission;
        public float ParticlesOrbitalMaxEmission;
        public Color ParticleColor;
    }


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

        _rsoCurrentChargeStep.onValueChanged += SetupParticleSettings;
    }

    void OnDisable()
    {
        isCharging = false;
        _energySphere.localScale = _minScale * Vector3.one;

        _rsoCurrentChargeStep.onValueChanged -= SetupParticleSettings;
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

    void SetupParticleSettings(int step)
    {
        ParticleSettingsData settings  = _listParticleSettingsData.FirstOrDefault(s => s.Step == step);

        var emission = _particleSfxChargeAttack.emission;
        emission.rateOverTime = settings.ParticlesEmission;

        var velocityOverLifetime = _particleSfxChargeAttack.velocityOverLifetime;
        velocityOverLifetime.speedModifier = settings.SpeedModifier;
        velocityOverLifetime.orbitalX = new ParticleSystem.MinMaxCurve(settings.ParticlesOrbitalMinEmission, settings.ParticlesOrbitalMaxEmission);
        velocityOverLifetime.orbitalY = new ParticleSystem.MinMaxCurve(settings.ParticlesOrbitalMinEmission, settings.ParticlesOrbitalMaxEmission);
        velocityOverLifetime.orbitalZ = new ParticleSystem.MinMaxCurve(settings.ParticlesOrbitalMinEmission, settings.ParticlesOrbitalMaxEmission);

        SetMidColor(_particleSfxChargeAttack, settings.ParticleColor);
    }

    public void SetMidColor(ParticleSystem ps, Color newColor)
    {
        var col = ps.colorOverLifetime;
        if (!col.enabled)
            return;

        Gradient grad = col.color.gradient;
        var colorKeys = grad.colorKeys;

        if (colorKeys == null || colorKeys.Length == 0)
            return;

        int bestIndex = 0;
        float bestDist = Mathf.Abs(colorKeys[0].time - 0.5f);

        for (int i = 1; i < colorKeys.Length; i++)
        {
            float d = Mathf.Abs(colorKeys[i].time - 0.5f);
            if (d < bestDist)
            {
                bestDist = d;
                bestIndex = i;
            }
        }

        const float epsilon = 0.01f;

        if (bestDist <= epsilon)
        {
            colorKeys[bestIndex].color = newColor;
        }
        else
        {
            var list = new List<GradientColorKey>(colorKeys);

            var midKey = new GradientColorKey(newColor, 0.5f);
            list.Add(midKey);

            list.Sort((a, b) => a.time.CompareTo(b.time));

            colorKeys = list.ToArray();
        }

        grad.colorKeys = colorKeys;
        col.color = new ParticleSystem.MinMaxGradient(grad);
    }
}