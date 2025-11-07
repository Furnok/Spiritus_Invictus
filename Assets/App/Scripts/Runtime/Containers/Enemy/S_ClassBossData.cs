using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class S_ClassBossData
{
    [Title("General Settings")]
    public float health = 0;

    public float walkSpeed = 0;

    public float runSpeed = 0;
}