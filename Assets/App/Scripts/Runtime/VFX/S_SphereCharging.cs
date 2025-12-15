using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_SphereCharging : MonoBehaviour
{
    //[Header("Sphere Settings")]
    //[SerializeField] private float _minScale = 0.2f;
    //[SerializeField] private float _maxScale = 1f;
    //[SerializeField] private float _pulseSpeed = 15f;
    //[SerializeField] private float _pulseAmplitude = 0.1f;

    [TabGroup("Settings")]
    [Title("Particle Parameters")]
    [SerializeField] private List<S_StructParticleSettingsData> _listParticleSettingsData = new();
    
    [TabGroup("References")]
    [Title("Transform")]
    [SerializeField] private Transform _energySphere;

    [TabGroup("References")]
    [Title("Particle")]
    [SerializeField] private ParticleSystem _particleSfxChargeAttack;

    [TabGroup("References")]
    [Title("MeshRenderer")]
    [SerializeField] private MeshRenderer _meshRendererEnergySphere;

    [TabGroup("Data")]
    [SerializeField] RSO_PlayerCurrentVisualProjectile _playerCurrentVisualProjectile;

    [TabGroup("Ouputs")]
    [SerializeField] private RSO_CurrentChargeStep _rsoCurrentChargeStep;

    [TabGroup("Ouputs")]
    [SerializeField] private SSO_PlayerAttackSteps _playerAttackSteps;

    [TabGroup("Ouputs")]
    [SerializeField] private SSO_PlayerStats _playerStats;

    //private float charge = 0f; // 0  1
    //private bool isCharging = true;

    S_StructParticleSettingsData _currentSettings;

    private Material _sphereMat = null;

    private Color _currentSphereColor = Color.white;
    private Color _targetSphereColor = Color.white;
    private float _currentSphereScale = 0;
    private float _targetSphereScale = 0;
    private float _colorLerpElapsed = 0f;
    private float _colorLerpDuration = 0f;

    private S_StructDataProjectileVisuals _currentVisualProjectileData;

    //private float _chargeDuration => GetMaxHoldTime();

    private void OnEnable()
    {
        //charge = 0f;
        //isCharging = true;

        _sphereMat = _meshRendererEnergySphere.material;

        _currentSettings = _listParticleSettingsData.FirstOrDefault(s => s.Step == 0);
        _currentSphereColor = _currentSettings.SphereColor;
        _targetSphereColor = _currentSettings.SphereColor;
        _colorLerpElapsed = 0f;
        _colorLerpDuration = 0f;
        _currentSphereScale = _currentSettings.ScaleEnergySphere;
        _targetSphereScale = _currentSettings.ScaleEnergySphere;

        _sphereMat.color = _currentSphereColor;

        _rsoCurrentChargeStep.onValueChanged += SetupParticleSettings;
    }

    private void OnDisable()
    {
        //isCharging = false;
        //_energySphere.localScale = _minScale * Vector3.one;

        _rsoCurrentChargeStep.onValueChanged -= SetupParticleSettings;
    }

    private void Update()
    {
        //if (isCharging)
        //    charge = Mathf.Clamp01(charge + Time.deltaTime / _chargeDuration);

        //float baseScale = Mathf.Lerp(_minScale, _maxScale, charge);
        //float pulse = 1f + Mathf.Sin(Time.time * _pulseSpeed) * _pulseAmplitude * charge;
        //float finalScale = baseScale * pulse;

        //_energySphere.localScale = Vector3.one * finalScale;

        if (_sphereMat != null)
        {
            if (_colorLerpDuration > 0f && _currentSphereColor != _targetSphereColor)
            {
                _colorLerpElapsed += Time.deltaTime;
                float t = Mathf.Clamp01(_colorLerpElapsed / _colorLerpDuration);

                Color c = Color.Lerp(_currentSphereColor, _targetSphereColor, t);
                float scale = Mathf.Lerp(_currentSphereScale, _targetSphereScale, t);

                _energySphere.localScale = Vector3.one * scale;

                _sphereMat.color = c;
            }
            else
            {
                _sphereMat.color = _targetSphereColor;

                _energySphere.localScale = Vector3.one * _targetSphereScale;
            }

            _currentVisualProjectileData.ScaleProjectile = _currentSphereScale;
            _currentVisualProjectileData.ColorProjectile = _currentSphereColor;

            _playerCurrentVisualProjectile.Value = _currentVisualProjectileData;
        }
    }

    private float GetMaxHoldTime()
    {
        float max = 0f;

        foreach (var step in _playerAttackSteps.Value)
        {
            if (step.timeHoldingInput > max) max = step.timeHoldingInput;
        }

        max += _playerStats.Value.timeWaitBetweenSteps * _playerAttackSteps.Value.Count - 1;

        return max;
    }

    private void SetupParticleSettings(int step)
    {
        S_StructParticleSettingsData settings  = _listParticleSettingsData.FirstOrDefault(s => s.Step == step);
        _currentSettings = settings;

        var stepData = _playerAttackSteps.Value.FirstOrDefault(a => a.step == step);
        var lastStepData = _playerAttackSteps.Value.FirstOrDefault(a => a.step == step - 1);

        var durationTransition = lastStepData.step != 0 ? stepData.timeHoldingInput - lastStepData.timeHoldingInput : stepData.timeHoldingInput;
        if (durationTransition > 0f) durationTransition += _playerStats.Value.timeWaitBetweenSteps;

        _colorLerpDuration = Mathf.Max(0.01f, durationTransition);
        _colorLerpElapsed = 0f;
        _currentSphereScale = _energySphere.localScale.x;
        
        _targetSphereScale = _currentSettings.ScaleEnergySphere;

        _currentSphereColor = _sphereMat != null ? _sphereMat.color : settings.SphereColor;
        _targetSphereColor = settings.SphereColor;

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
        if (!col.enabled) return;

        Gradient grad = col.color.gradient;
        var colorKeys = grad.colorKeys;

        if (colorKeys == null || colorKeys.Length == 0) return;

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

        if (bestDist <= epsilon) colorKeys[bestIndex].color = newColor;
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