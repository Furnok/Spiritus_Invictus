using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public struct S_StructProjectileData
{
    [Title("Data")]
    public Vector3 locationSpawn;

    public Vector3 direction;
}
