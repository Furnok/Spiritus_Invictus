using System.Collections;
using UnityEngine;

public class S_TimeManager : MonoBehaviour
{
    [Header("Settings")]
    [Header("Dodge Perfect Configuration")]
    [SerializeField, Range(0f, 1f)] private float _slowScale = 0.25f;
    [SerializeField] private float _hitStop = 0.06f;
    [SerializeField] private float _slowDuration = 0.5f;
    [SerializeField] private float _blendOut = 0.35f;
    [SerializeField] private bool _scaleFixedDelta = true;



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
    }

    void OnDisable()
    {
        _rseOnDodgePerfect.action -= StartPerfectDodgeSlowMotion;
        _rseOnGamePause.action -= PauseValueChange;

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
        _gameTimeScale = _slowScale;
        ApplyTimeScale();

        float elapsed = 0f;
        while (elapsed < _slowDuration)
        {
            if (!_rsoGameInPause.Value)
                elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        float t = 0f;
        while (t < _blendOut)
        {
            if (!_rsoGameInPause.Value)
            {
                t += Time.unscaledDeltaTime;
                float k = t / _blendOut;
                _gameTimeScale = Mathf.Lerp(_slowScale, 1f, k);
                ApplyTimeScale();
            }
            yield return null;
        }

        _gameTimeScale = 1f;
        ApplyTimeScale();
        _slowMoCo = null;
    }

   
}