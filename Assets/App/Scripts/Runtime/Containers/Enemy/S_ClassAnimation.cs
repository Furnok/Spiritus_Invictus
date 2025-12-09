using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class S_ClassAnimation
{
    [Title("Animation")]
    public AnimationClip animation = null;

    [Title("Animation Root")]
    [Range(1, 100)] public float rootMotionMultiplier = 1;

    [Title("Data")]
    public S_StructEnemyAttackData attackData;
}