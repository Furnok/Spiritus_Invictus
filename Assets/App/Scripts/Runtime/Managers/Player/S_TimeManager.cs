using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class S_TimeManager : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("General")]
    [SerializeField, Range(0f, 1f)] private float _slowScaleDodge = 0.25f;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float _hitStopDodge = 0.06f;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float _slowDurationDodge = 0.5f;

    [TabGroup("Settings")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float _blendOutDodge = 0.35f;

    [TabGroup("Settings")]
    [Title("Parry Configuration")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float _hitStopParry = 0.1f;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerDodgePerfect _rseOnDodgePerfect;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnGamePause _rseOnGamePause;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnParrySuccess _rseOnParrySuccess;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_GameInPause _rsoGameInPause;

    private float _baseFixedDelta = 0;
    private float _gameTimeScale = 1f;

    private Coroutine _slowMoCo = null;

    private void Awake()
    {
        _baseFixedDelta = Time.fixedDeltaTime;
        _gameTimeScale = 1f;

        _gameTimeScale = 1f;
        PauseValueChange(false);
    }

    private void OnEnable()
    {
        _rseOnDodgePerfect.action += StartPerfectDodgeSlowMotion;
        _rseOnGamePause.action += PauseValueChange;
        _rseOnParrySuccess.action += TriggerOnParryCoroutine;
    }

    private void OnDisable()
    {
        _rseOnDodgePerfect.action -= StartPerfectDodgeSlowMotion;
        _rseOnGamePause.action -= PauseValueChange;
        _rseOnParrySuccess.action -= TriggerOnParryCoroutine;

        if (_slowMoCo != null)
        {
            StopCoroutine(_slowMoCo);
            _slowMoCo = null;
        }

        _gameTimeScale = 1f;
        _rsoGameInPause.Value = false;
        ApplyGameplayTimeScale();
    }

    private void PauseValueChange(bool newPauseState)
    {
        _rsoGameInPause.Value = newPauseState;
        ApplyGameplayTimeScale();
    }

    private void ApplyGameplayTimeScale()
    {
        float effective = _rsoGameInPause.Value ? 0f : _gameTimeScale;
        Time.timeScale = effective;
        Time.fixedDeltaTime = _baseFixedDelta * Mathf.Max(effective, 0.01f);
    }

    private void StartPerfectDodgeSlowMotion()
    {
        if (_slowMoCo != null) StopCoroutine(_slowMoCo);
        _slowMoCo = StartCoroutine(CoroutineSlowMotion());
    }

    private IEnumerator CoroutineSlowMotion()
    {
        if (_hitStopDodge > 0f)
        {
            _gameTimeScale = 0f;
            ApplyGameplayTimeScale();

            float hsElapsed = 0f;
            while (hsElapsed < _hitStopDodge)
            {
                if (!_rsoGameInPause.Value)
                    hsElapsed += Time.unscaledDeltaTime;

                yield return null;
            }
        }

        _gameTimeScale = _slowScaleDodge;
        ApplyGameplayTimeScale();

        float elapsed = 0f;
        while (elapsed < _slowDurationDodge)
        {
            if (!_rsoGameInPause.Value)
                elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        float t = 0f;
        while (t < _blendOutDodge)
        {
            if (!_rsoGameInPause.Value)
            {
                t += Time.unscaledDeltaTime;
                float k = t / _blendOutDodge;
                _gameTimeScale = Mathf.Lerp(_slowScaleDodge, 1f, k);
                ApplyGameplayTimeScale();
            }
            yield return null;
        }

        _gameTimeScale = 1f;
        ApplyGameplayTimeScale();
        _slowMoCo = null;
    }

    private void TriggerOnParryCoroutine (S_StructAttackContact contact) =>  StartCoroutine(CoroutineOnParry());

    private IEnumerator CoroutineOnParry()
    {
        if (_hitStopParry <= 0f) yield break;

        float previousGameScale = _gameTimeScale;

        _gameTimeScale = 0f;
        ApplyGameplayTimeScale();

        float elapsed = 0f;
        while (elapsed < _hitStopParry)
        {
            if (!_rsoGameInPause.Value)
                elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        _gameTimeScale = previousGameScale;
        ApplyGameplayTimeScale();
    }
}