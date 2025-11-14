using System.Collections;
using UnityEngine;

public class S_TimeManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float _slowScaleDodge = 0.25f;
    [SerializeField] private float _hitStopDodge = 0.06f;
    [SerializeField] private float _slowDurationDodge = 0.5f;
    [SerializeField] private float _blendOutDodge = 0.35f;

    [Header("Parry Configuration")]
    [SerializeField] private float _hitStopParry = 0.1f;


    [Header("References")]
    [SerializeField] RSO_GameInPause _rsoGameInPause;


    [Header("Inputs")]
    [SerializeField] RSE_OnParrySuccess _rseOnParrySuccess;
    [SerializeField] RSE_OnPlayerDodgePerfect _rseOnDodgePerfect;
    [SerializeField] RSE_OnGamePause _rseOnGamePause;
    //[Header("Outputs")]


    float _baseFixedDelta;
    float _gameTimeScale = 1f;
    Coroutine _slowMoCo;


    private void Awake()
    {
        _baseFixedDelta = Time.fixedDeltaTime;
        _gameTimeScale = 1f;
        ApplyTimeScale();
    }

    void OnEnable()
    {
        _rseOnDodgePerfect.action += StartPerfectDodgeSlowMotion;
        _rseOnGamePause.action += PauseValueChange;
        _rseOnParrySuccess.action += TriggerOnParryCoroutine;

    }

    void OnDisable()
    {
        _rseOnDodgePerfect.action -= StartPerfectDodgeSlowMotion;
        _rseOnGamePause.action -= PauseValueChange;
        _rseOnParrySuccess.action -= TriggerOnParryCoroutine;
        _rsoGameInPause.Value = false;
        ApplyTimeScale();
    }

    void PauseValueChange(bool newPauseState)
    {
        ApplyTimeScale();
    }


    void ApplyTimeScale()
    {
        float effective = _rsoGameInPause.Value ? 0f : _gameTimeScale;
        Time.timeScale = effective;
        Time.fixedDeltaTime = _baseFixedDelta * Mathf.Max(effective, 0.01f);
    }

    void StartPerfectDodgeSlowMotion()
    {
        if (_slowMoCo != null) StopCoroutine(_slowMoCo);
        _slowMoCo = StartCoroutine(CoroutineSlowMotion());
    }

    IEnumerator CoroutineSlowMotion()
    {
        if (_hitStopDodge > 0f)
        {
            _gameTimeScale = 0f;
            ApplyTimeScale();

            float hsElapsed = 0f;
            while (hsElapsed < _hitStopDodge)
            {
                if (!_rsoGameInPause.Value)
                    hsElapsed += Time.unscaledDeltaTime;

                yield return null;
            }
        }

        _gameTimeScale = _slowScaleDodge;
        ApplyTimeScale();

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
                ApplyTimeScale();
            }
            yield return null;
        }

        _gameTimeScale = 1f;
        ApplyTimeScale();
        _slowMoCo = null;
    }

    void TriggerOnParryCoroutine (AttackContact contact) =>  StartCoroutine(CoroutineOnParry());
    IEnumerator CoroutineOnParry()
    {
        if (_hitStopParry <= 0f) yield break;

        float previousGameScale = _gameTimeScale;

        _gameTimeScale = 0f;
        ApplyTimeScale();

        float elapsed = 0f;
        while (elapsed < _hitStopParry)
        {
            if (!_rsoGameInPause.Value)
                elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        _gameTimeScale = previousGameScale;
        ApplyTimeScale();
    }
}