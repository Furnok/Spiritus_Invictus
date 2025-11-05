using UnityEngine;
using System;

[Serializable]
public struct S_StructEnemyAttackData
{
    public int attackId;
    [HideInInspector]public int goSourceId;
    public S_EnumEnemyAttackType attackType;
    public float damage;
    public float knockbackHitDuration; //maybe remove it if hit stun is same as knockback duration who make sense 
    public float knockbackHitDistance;
    public float knockbackOnParryDuration;
    public float knockbackOnParrryDistance;
    public float parryToleranceBeforeHit;
    public float parryToleranceAfterHit;
    //public float hitStunDuration;
    public float invicibilityDuration;
    public float convictionReduction;
    [HideInInspector] public Vector3 attackDirection;
}
