using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class S_ClassAnimation
{
    [Title("Animation")]
    public AnimationClip animation = null;

    [Title("Data")]
    public S_StructEnemyAttackData attackData;
}