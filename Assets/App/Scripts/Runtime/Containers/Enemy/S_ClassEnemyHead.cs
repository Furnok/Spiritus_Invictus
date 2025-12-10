using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class S_ClassEnemyHead
{
    [Title("Data")]
    [Range(0f, 1f)] public float weight = 1f;

    [Range(0f, 1f)] public float bodyWeight = 0f;

    [Range(0f, 1f)] public float headWeight = 1f;

    [Range(0f, 1f)] public float eyesWeight = 0f;

    [Range(0f, 1f)] public float clampWeight = 0.5f;
}