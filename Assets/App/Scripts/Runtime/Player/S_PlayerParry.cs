using UnityEngine;

public class S_PlayerParry : MonoBehaviour
{
    //[Header("Settings")]
    

    [Header("References")]
    [SerializeField] RSO_CanParry _canParry;
    [SerializeField] RSO_ParryStartTime _parryStartTime;
    [SerializeField] SSO_PlayerStats _playerStats;

    [Header("Input")]
    [SerializeField] private RSE_OnPlayerParryInput rseOnPlayerParry;

    //[Header("Output")]

    float _parryDuration => _playerStats.Value.parryDuration;



    private void Awake()
    {
        _canParry.Value = false;
        _parryStartTime.Value = 0f;
    }
    private void OnEnable()
    {
        rseOnPlayerParry.action += TryParry;
    }

    private void OnDisable()
    {
        rseOnPlayerParry.action -= TryParry;
    }

    private void TryParry()
    {
        //if can parry

        if (true)
        {
            _parryStartTime.Value = Time.time;
            _canParry.Value = true;

            StartCoroutine(S_Utils.Delay(_parryDuration, () =>
            {
                _canParry.Value = false;
            }));
        }
    }
}