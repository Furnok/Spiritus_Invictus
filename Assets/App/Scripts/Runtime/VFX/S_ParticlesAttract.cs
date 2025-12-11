using UnityEngine;

public class S_ParticlesAttract : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float attractStartLife = 0.1f;           // at 70% of lifetime atrtract to the target transform
    [SerializeField] private float attractionStrength = 8f;
    [SerializeField] private float attractionLerp = 10f;
    [SerializeField] private float killRadius = 0.1f;

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private ParticleSystem _ps;

    //[Header("Inputs")]

    [Header("Outputs")]
    [SerializeField] private RSE_OnPlayerGainConviction _onPlayerGainConviction;

    private ParticleSystem.Particle[] _particles;
    private float _ammountTotalConvictionGain = 0f;
    private int _totalParticles = 0;
    private float _convictionPerParticle => _totalParticles > 0 ? _ammountTotalConvictionGain / _totalParticles : 0;

    public void InitializeTransform(Transform transformToAttract, float ammountConvictionGain)
    {
        _ps.Play();
        target = transformToAttract;
        _ammountTotalConvictionGain = ammountConvictionGain;
        _totalParticles = _ps.emission.burstCount > 0 ? (int)_ps.emission.GetBurst(0).count.constant : 0;
        Debug.Log("Total Particles Emitted: " + _totalParticles);
    }

    void LateUpdate()
    {
        if (target == null) return;

        int max = _ps.main.maxParticles;
        if (_particles == null || _particles.Length < max)
        {
            _particles = new ParticleSystem.Particle[max];
        }

        int count = _ps.GetParticles(_particles);
        if (count == 0) return;

        bool worldSim = _ps.main.simulationSpace == ParticleSystemSimulationSpace.World;

        for (int i = 0; i < count; i++)
        {
            var p = _particles[i];

            float life01 = 1f - (p.remainingLifetime / p.startLifetime);

            if (life01 < attractStartLife)
            {
                _particles[i] = p;
                continue;
            }

            Vector3 worldPos = worldSim
                ? p.position
                : transform.TransformPoint(p.position);

            Vector3 toTarget = target.position - worldPos;
            float dist = toTarget.magnitude;
            if (dist < killRadius)
            {
                p.remainingLifetime = 0f;
                _particles[i] = p;
                _onPlayerGainConviction.Call(_convictionPerParticle);
                continue;
            }

            toTarget /= Mathf.Max(dist, 0.0001f);

            Vector3 velWorld = worldSim
                ? p.velocity
                : transform.TransformDirection(p.velocity);

            Vector3 desiredVel = toTarget * attractionStrength;
            velWorld = Vector3.Lerp(velWorld, desiredVel, Time.deltaTime * attractionLerp);

            p.velocity = worldSim
                ? velWorld
                : transform.InverseTransformDirection(velWorld);

            _particles[i] = p;
        }

        _ps.SetParticles(_particles, count);

        if (count == 0)
        {
            Destroy(gameObject);
        }
    }
}