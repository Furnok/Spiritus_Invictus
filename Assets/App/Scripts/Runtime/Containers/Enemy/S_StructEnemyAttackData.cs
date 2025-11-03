using UnityEngine;
using System;

[Serializable]
public struct S_StructEnemyAttackData
{
    public int attackId;
    [HideInInspector]public int goSourceId;
    public S_EnumEnemyAttackType attackType;
    public float damage;
    public float knockbackForce;
    public float convictionReduction;
}
