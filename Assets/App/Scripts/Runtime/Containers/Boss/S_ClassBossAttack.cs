using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[Serializable]
public class S_ClassBossAttack
{
    [Title("Attack Settings")]
    public string attackName = "";

    [SuffixLabel("s", Overlay = true)]
    public float attackTime = 0;

    public bool isAttackDistance = false;

    public float pvBossUnlock = 0;

    public float difficultyLevel = 0;

    [Title("Combo Settings")]
    public List<S_ClassAnimation> listComboData = new();
}