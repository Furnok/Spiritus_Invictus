using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public struct S_StructEnemyInfo
{
    [Title("Id")]
    public int enemyID;

    [Title("Position")]
    public Vector3 lastPostion;
}