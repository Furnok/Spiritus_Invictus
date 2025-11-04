using UnityEngine;
using System;

[Serializable]
public struct ProjectileData
{
    [Header("Arc Settings")]
    [Tooltip("Arc height factor 1 = average, 2 = hight")]
    public float arcHeightMultiplier;
    [Tooltip("Curve direction : 0=top, 1=right diagonal, -1=left diagonal")]
    public float arcDirection;
    [Tooltip("Makes the trajectory random")]
    public bool randomizeArc;
    [Tooltip("Min arc direction if random")]
    [Range(-5, 5)] public float arcRandomDirectionMin;
    [Tooltip("Max arc direction if random")]
    [Range(-5, 5)] public float arcRandomDirectionMax;
    [Tooltip("How long does it take for the projectile to reach the target (s)?")]
    public float travelTime;
    [Tooltip("the speed during the travel in each moment base on travel time duration")]
    public AnimationCurve speedAnimationCurve; //Not implemented yet

    [Header("Projectile Visual")]
    public float trailTime; //Not implemented yet
    public float trailSize; //Not implemented yet
    public Color trailColor; //Not implemented yet
}
