using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[Serializable]
public class S_ClassEnemyData
{
    [Title("General Settings")]
    public float health;

    public float speed;

    [Title("Combat")]
    public float attackHeavyDamage;

    public float attackLightDamage;

    public float projectileDamage;

    [SuffixLabel("s", Overlay = true)]
    public float projectileCooldown;

    public float detectionRange;

    public float detectionAggroRangeMax;

    public float distanceToChase;

    public float distanceToLoseAttack;

    [SuffixLabel("s", Overlay = true)]
    public float attackCooldown;

    [SuffixLabel("s", Overlay = true)]
    public float timeDespawn;

    [Title("Secret")]
    [SuffixLabel("%", Overlay = true)]
    public float chanceForEasterEgg;

    [Title("Delay")]
    [SuffixLabel("s", Overlay = true)]
    public float patrolPointWaitMin;

    [SuffixLabel("s", Overlay = true)]
    public float patrolPointWaitMax;

    [SuffixLabel("s", Overlay = true)]
    public float startPatrolWaitMin;

    [SuffixLabel("s", Overlay = true)]
    public float startPatrolWaitMax;

    [SuffixLabel("s", Overlay = true)]
    public float timeBeforeChaseMin;

    [SuffixLabel("s", Overlay = true)]
    public float timeBeforeChaseMax;

    [Title("Animations")]
    public List<S_ClassAnimationsCombos> listCombos;
}