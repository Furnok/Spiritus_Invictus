using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public struct S_StructDataProjectile
{
    [Title("Arc Settings")]
    public float arcHeightMultiplier;

    public float arcDirection;

    public bool randomizeArc;

    [Range(-5, 5)] public float arcRandomDirectionMin;

    [Range(-5, 5)] public float arcRandomDirectionMax;

    [Title("Time")]
    [SuffixLabel("s", Overlay = true)]
    public float travelTime;

    [Title("Curve")]
    public AnimationCurve speedAnimationCurve;

    [Title("Projectile Visual")]
    public float trailTime;

    public float trailSize;

    public Color trailColor;
}