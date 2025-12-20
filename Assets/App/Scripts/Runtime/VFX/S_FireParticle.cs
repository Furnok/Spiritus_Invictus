using Sirenix.OdinInspector;
using UnityEngine;

public class S_FireParticle : MonoBehaviour
{
    [TabGroup("References")]
    [SerializeField] private ParticleSystem _particleSystem;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayParticle rseOnPlayParticle;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnStopParticle rseOnStopParticle;

    private void OnEnable()
    {
        rseOnPlayParticle.action += PlayParticle;
        rseOnStopParticle.action += StopParticle;
    }
    private void OnDisable()
    {
        rseOnPlayParticle.action -= PlayParticle;
        rseOnStopParticle.action -= StopParticle;
    }
    private void StopParticle()
    {
        _particleSystem.Stop();
    }

    private void PlayParticle()
    {
        _particleSystem.Play();
    }
}