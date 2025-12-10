using System.Collections.Generic;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class S_ClassBossData
{
    [Title("General Settings")]
    public float healthPhase1 = 0;

    public float healthPhase2 = 0;

    public float walkSpeed = 0;

    public float runSpeed = 0;

    public float distanceToChase = 0;

    [SuffixLabel("%", Overlay = true)]
    public float distanceToRun = 0;

    [Title("Animations")]
    public List<S_ClassBossAttack> listAttackPhase1 = new();

    public List<S_ClassBossAttack> listAttackPhase2 = new();
}