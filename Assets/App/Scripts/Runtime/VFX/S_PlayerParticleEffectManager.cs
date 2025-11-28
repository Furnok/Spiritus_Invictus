using Sirenix.OdinInspector;
using UnityEngine;

public class S_PlayerParticleEffectManager : MonoBehaviour
{
    [Header("Settings")]
    [Title("Offsets")]
    [SerializeField] private float _forwardOffset = 0.5f;

    [Header("Settings")]
    [SerializeField] private float _upwardOffset = 0.2f;

    [Header("References")]
    [Title("Parent")]
    [SerializeField] private Transform _particleEffectParent;

    [Header("References")]
    [Title("Prefab")]
    [SerializeField] private GameObject _prefabParryEffect;

    [Header("Inputs")]
    [SerializeField] private RSE_OnParrySuccess _onParrySuccess;

    private void OnEnable()
    {
        _onParrySuccess.action += ActiveParryEffect;
    }

    private void OnDisable()
    {
        _onParrySuccess.action -= ActiveParryEffect;
    }

    private void ActiveParryEffect(S_StructAttackContact contact)
    {
        Vector3 offset = transform.forward * _forwardOffset + transform.up * _upwardOffset;

        var spawnPoint = contact.data.contactPoint + offset;

        var parryeffect = Instantiate(_prefabParryEffect, spawnPoint, _particleEffectParent.rotation);

        Destroy(parryeffect, 2f);
    }
}