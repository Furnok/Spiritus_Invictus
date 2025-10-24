using UnityEngine;
using System;

[Serializable]
public struct EnemyAttackData
{
    public int attackId;
    [HideInInspector]public int goSourceId;
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
