using UnityEngine;

public class S_ParticlesAttract : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Range(0f, 1f)] private float attractStartLife01 = 0.7f;           // at 70% of lifetime atrtract to the target transform
    [SerializeField] private float attractionStrength = 8f;
    [SerializeField] private float attractionLerp = 10f;
    [SerializeField] private float killRadius = 0.1f;

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private ParticleSystem _ps;

    //[Header("Inputs")]

    //[Header("Outputs")]

    private ParticleSystem.Particle[] _particles;

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

            if (life01 < attractStartLife01)
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
    }
}