using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public struct S_StructAttackContact
{
    [Title("Data")]
    public S_StructEnemyAttackData data;

    public Collider source;
}
