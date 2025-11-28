using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public struct S_StructPlayerStatsData
{
    [Title("Player values")]
    public float maxHealth;

    public float healAmount;

    [SuffixLabel("s", Overlay = true)]
    public float delayBeforeHeal;

    [Title("Movement player values")]
    public float moveSpeed;

    public float strafeSpeed;

    public float turnSpeed;

    public float turnSpeedTargeting;

    public float runSpeed;

    [SuffixLabel("s", Overlay = true)]
    public float delayBeforeRunningAfterDodge;

    public float maxSlopeAngle;

    [Title("Dodge values")]
    [SuffixLabel("s", Overlay = true)]
    public float dodgeDuration;

    public AnimationCurve _speedDodgeCurve;

    public float dodgeDistance;

    public float dodgeCooldown;

    [Title("Parry values")]
    [SuffixLabel("s", Overlay = true)]
    public float parryDuration;

    [SuffixLabel("s", Overlay = true)]
    public float parryCooldown;

    [Title("Attack values")]
    [SuffixLabel("s", Overlay = true)]
    public float projectileLifeTime;

    [SuffixLabel("s", Overlay = true)]
    public float delayBeforeCastAttack;

    [SuffixLabel("s", Overlay = true)]
    public float timeWaitBetweenSteps;
}