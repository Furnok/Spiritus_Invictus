using UnityEngine;
using System;

[Serializable]
public struct AttackData
{
    public EnemyAttackType attackType;
    public float damage;
    public float knockbackForce;
    public float convictionDamage;
    public bool canBeParried;
    public bool canBeDodged;
    public bool unblockable;
}

public enum EnemyAttackType
{
    Dodgeable,
    Parryable,
    Projectile,
}
