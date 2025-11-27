using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[Serializable]
public class S_ClassAnimationsCombos
{
    [Title("Combos")]
    public List<S_ClassAnimation> listAnimationsCombos = new();

    [Title("Distances")]
    public float distanceToChase = 0;
    public float distanceToLoseAttack = 0;
}