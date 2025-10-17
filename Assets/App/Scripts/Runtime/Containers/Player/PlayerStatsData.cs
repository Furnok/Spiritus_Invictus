using System;
using UnityEngine;

[Serializable]
public struct PlayerStatsData
{
    [Header("Player values")]
    public float maxHealth;
    public float healAmount;
    public float delayBeforeHeal;

    [Header("Movement player values")]
    public float moveSpeed;
    public float strafeSpeed;
    public float turnSpeed;
    public float turnSpeedTargeting;

    [Header("Dodge values")]
    public float dodgeForce;
    public float dodgeDuration;
    public AnimationCurve _speedDodgeCurve;

    [Header("Parry values")]
    public float parryToleranceBeforeHit;
    public float parryToleranceAfterHit;
    public float parryDuration;

    [Header("Attack values")]
    public float baseDamage;

}
