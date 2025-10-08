using UnityEngine;
using System;

[Serializable]
public struct AttackData
{
    public EnemyAttackType attackType;
    public float damage;
    public float knockbackForce;
    public float convictionReduction;
}

public enum EnemyAttackType
{
    Dodgeable,
    Parryable,
    Projectile,
}
