using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class S_ClassAnimationsCombos
{
    [Title("Combos")]
    public List<AnimationClip> listAnimationsCombos;

    [Title("Type")]
    public bool isProjectile;
}