using UnityEngine;
using System;

[Serializable]
public struct S_StructEnemyAttackData
{
    public int attackId;
    [HideInInspector]public int goSourceId;
    public S_EnumEnemyAttackType attackType;
    public float damage;
    public float knockbackDuration; //maybe remove it if hit stun is same as knockback duration who make sense 
    public float knockbackDistance;
    public float parryToleranceBeforeHit;
    public float parryToleranceAfterHit;
    public float hitStunDuration;
    public float convictionReduction;

}
