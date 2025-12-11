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
    [SerializeField] private Transform _targetAttract;
    [SerializeField] private Transform _convictionDodgeEffectParent;


    [Header("References")]
    [Title("Stats")]
    [SerializeField] private SSO_PlayerStats _playerStats;
    [SerializeField] private SSO_PlayerConvictionData _playerConvictionData;

    [Header("References")]
    [Title("Prefab")]
    [SerializeField] private GameObject _prefabParryEffect;
    [SerializeField] private S_ParticlesAttract _prefabParticlesAttractParryGain;
    [SerializeField] private S_ParticlesAttract _prefabParticlesAttractDodgeGain;
    //[SerializeField] private GameObject _prefabChargingEffect;

    [Header("Inputs")]
    [SerializeField] private RSE_OnParrySuccess _onParrySuccess;
    [SerializeField] private RSE_OnAttackStartPerformed _onAttackStartPerformed;
    [SerializeField] private RSE_OnPlayerGettingHit _rseOnPlayerGettingHit;
    [SerializeField] private RSE_OnSpawnProjectile rseOnSpawnProjectile;
    [SerializeField] private RSE_OnPlayerDodgePerfect _rseOnDodgePerfect;


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
        _rseOnDodgePerfect.action += ActiveDodgeEffect;
    }

    private void OnDisable()
    {
        _onParrySuccess.action -= ActiveParryEffect;

        _onAttackStartPerformed.action -= ActiveChargeEffect;
        _rseOnPlayerGettingHit.action -= DeactiveChargeEffect;
        rseOnSpawnProjectile.action -= DeactiveChargeEffect;
        _rseOnDodgePerfect.action -= ActiveDodgeEffect;
    }

    private void ActiveParryEffect(S_StructAttackContact contact)
    {
        Vector3 offset = transform.forward * _forwardOffsetParry + transform.up * _upwardOffsetParry;

        var spawnPoint = contact.data.contactPoint + offset;

        var parryeffect = Instantiate(_prefabParryEffect, spawnPoint, Quaternion.identity, _particleEffectParent);
        var attract = Instantiate(_prefabParticlesAttractParryGain, spawnPoint, _targetAttract.rotation, _targetAttract);

        attract.InitializeTransform(_targetAttract, contact.data.convictionParryGain);

        Destroy(parryeffect, 2f);
    }

    private void ActiveDodgeEffect()
    {
        var attract = Instantiate(_prefabParticlesAttractDodgeGain, _convictionDodgeEffectParent.position, _convictionDodgeEffectParent.rotation, _convictionDodgeEffectParent);

        attract.InitializeTransform(_targetAttract, _playerConvictionData.Value.dodgeSuccessGain);
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