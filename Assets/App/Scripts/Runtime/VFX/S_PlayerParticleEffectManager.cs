using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerParticleEffectManager : MonoBehaviour
{
    [Header("Settings")]
    [Title("Offsets")]
    [SerializeField] private float _forwardOffsetParry = 0.5f;

    [Header("Settings")]
    [SerializeField] private float _upwardOffsetParry = 0.2f;

    [Header("References")]
    [Title("Parent")]
    [SerializeField] private Transform _particleEffectParent;
    [SerializeField] private Transform _chargingEffectParent;

    [Header("References")]
    [Title("Stats")]
    [SerializeField] private SSO_PlayerStats _playerStats;

    [Header("References")]
    [Title("Prefab")]
    [SerializeField] private GameObject _prefabParryEffect;
    //[SerializeField] private GameObject _prefabChargingEffect;

    [Header("Inputs")]
    [SerializeField] private RSE_OnParrySuccess _onParrySuccess;
    [SerializeField] private RSE_OnAttackStartPerformed _onAttackStartPerformed;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;
    [SerializeField] private RSE_OnSpawnProjectile rseOnSpawnProjectile;

    private void Awake()
    {
        _chargingEffectParent.localPosition += _playerStats.Value.attackOffset;
    }

    private void OnEnable()
    {
        _onParrySuccess.action += ActiveParryEffect;

        _onAttackStartPerformed.action += ActiveChargeEffect;
        _rseOnPlayerGettingHit.action += DeactiveChargeEffect;
        rseOnSpawnProjectile.action += DeactiveChargeEffect;
    }

    private void OnDisable()
    {
        _onParrySuccess.action -= ActiveParryEffect;

        _onAttackStartPerformed.action -= ActiveChargeEffect;
        _rseOnPlayerGettingHit.action -= DeactiveChargeEffect;
        rseOnSpawnProjectile.action -= DeactiveChargeEffect;
    }

    private void ActiveParryEffect(S_StructAttackContact contact)
    {
        Vector3 offset = transform.forward * _forwardOffsetParry + transform.up * _upwardOffsetParry;

        var spawnPoint = contact.data.contactPoint + offset;

        var parryeffect = Instantiate(_prefabParryEffect, spawnPoint, _particleEffectParent.rotation);

        Destroy(parryeffect, 2f);
    }

    void ActiveChargeEffect()
    {
        _chargingEffectParent.gameObject.SetActive(true);
    }

    void DeactiveChargeEffect()
    {
        _chargingEffectParent.gameObject.SetActive(false);
    }

    void DeactiveChargeEffect(float value)
    {
        _chargingEffectParent.gameObject.SetActive(false);
    }
}