using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class S_ClassEnemyData
{
    [Title("General Settings")]
    public float health = 0;

    public float speedPatrol = 0;

    public float speedChase = 0;

    public float speedStrafe = 0;

    [Title("Strafe")]
    [SuffixLabel("s", Overlay = true)]
    public float strafeWaitTimeMin = 0;

    [SuffixLabel("s", Overlay = true)]
    public float strafeWaitTimeMax = 0;

    [SuffixLabel("°", Overlay = true)]
    public float strafeRotationMin = 0;

    [SuffixLabel("°", Overlay = true)]
    public float strafeRotationMax = 0;

    [Title("Combat")]
    public float detectionRange = 0;

    public float detectionAggroRangeMax = 0;

    [SuffixLabel("s", Overlay = true)]
    public float attackCooldown = 0;

    [SuffixLabel("s", Overlay = true)]
    public float waitStun = 0;

    [Title("Delay")]
    [SuffixLabel("s", Overlay = true)]
    public float patrolPointWaitMin = 0;

    [SuffixLabel("s", Overlay = true)]
    public float patrolPointWaitMax = 0;

    [SuffixLabel("s", Overlay = true)]
    public float startPatrolWaitMin = 0;

    [SuffixLabel("s", Overlay = true)]
    public float startPatrolWaitMax = 0;

    [SuffixLabel("s", Overlay = true)]
    public float timeBeforeChaseMin = 0;

    [SuffixLabel("s", Overlay = true)]
    public float timeBeforeChaseMax = 0;

    [Title("Animations")]
    public AnimatorOverrideController controllerOverride = null;

    public List<S_ClassAnimationsCombos> listCombos = new();
}