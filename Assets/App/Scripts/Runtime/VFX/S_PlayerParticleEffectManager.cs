using System.Collections.Generic;
using UnityEngine;

public class S_PlayerParticleEffectManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float _forwardOffset = 0.5f;
    [SerializeField] float _upwardOffset = 0.2f;
    [Header("References")]
    [SerializeField] Transform _particleEffectParent;
    [SerializeField] GameObject _prefabParryEffect;

    [Header("Inputs")]
    [SerializeField] RSE_OnParrySuccess _onParrySuccess;

    //[Header("Outputs")]

    private void OnEnable()
    {
        _onParrySuccess.action += ActiveParryEffect;
    }

    private void OnDisable()
    {
        _onParrySuccess.action -= ActiveParryEffect;
    }
    void ActiveParryEffect(AttackContact contact)
    {
        Vector3 offset = transform.forward * _forwardOffset + transform.up * _upwardOffset;

        var spawnPoint = contact.data.contactPoint + offset;

        var parryeffect = Instantiate(_prefabParryEffect, spawnPoint, _particleEffectParent.rotation);

        Destroy(parryeffect, 2f);
    }
}