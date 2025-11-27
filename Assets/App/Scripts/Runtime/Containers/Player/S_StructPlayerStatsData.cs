using System;
using UnityEngine;

[Serializable]
public struct S_StructPlayerStatsData
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
    public float runSpeed;
    public float delayBeforeRunningAfterDodge;
    public float maxSlopeAngle;

    [Header("Dodge values")]
    public float dodgeDuration;
    public AnimationCurve _speedDodgeCurve;
    public float dodgeDistance;
    public float dodgeCooldown;

    [Header("Parry values")]
    public float parryDuration;
    public float parryCooldown;

    [Header("Attack values")]
    public float projectileLifeTime;
    public float delayBeforeCastAttack;
    public float timeWaitBetweenSteps;

}
