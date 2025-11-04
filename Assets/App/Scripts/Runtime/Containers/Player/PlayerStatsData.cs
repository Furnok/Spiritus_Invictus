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
    public float runSpeed;
    public float delayBeforeRunningAfterDodge;

    [Header("Dodge values")]
    public float dodgeForce;
    public float dodgeDuration;
    public AnimationCurve _speedDodgeCurve;
    public float dodgeDistance; //not implemented
    public float maxSlopeAngle; //not implemented

    [Header("Parry values")]
    //public float parryToleranceBeforeHit;
    //public float parryToleranceAfterHit;
    public float parryDuration;

    [Header("Attack values")]
    public float projectileLifeTime;
    public float delayBeforeCastAttack;
    public float timeWaitBetweenSteps;


    [Header("Getting hit values")]
    public float hitStunDuration;
}
