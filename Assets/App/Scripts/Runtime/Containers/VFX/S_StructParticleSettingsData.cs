using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public struct S_StructParticleSettingsData
{
    [Title("Data")]
    public int Step;

    public float SpeedModifier;

    public float ParticlesEmission;

    public float ParticlesOrbitalMinEmission;

    public float ParticlesOrbitalMaxEmission;

    public Color ParticleColor;

    public Color SphereColor;

    public float ScaleEnergySphere;
}
