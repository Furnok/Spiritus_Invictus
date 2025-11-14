using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class S_ClassBossAttack
{
    [Title("Attack Settings")]
    public string attackName;

    public bool isAttackDistance;

    public float pvBossUnlock;

    public float difficultyLevel;

    public List<S_StructEnemyAttackData> listComboData;
}