using Sirenix.OdinInspector;
using System;

[Serializable]
public class S_ClassAttackOwned
{
    [Title("Attack Data")]
    public S_ClassBossAttack bossAttack = null;

    [Title("Settings")]
    public float frequency = 0;

    public float score = 0;
}