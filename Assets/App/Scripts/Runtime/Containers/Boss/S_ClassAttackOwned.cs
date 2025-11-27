using Sirenix.OdinInspector;
using System;

[Serializable]
public class S_ClassAttackOwned
{
    [Title("Attack Settings")]
    public S_ClassBossAttack bossAttack = null;
    public float frequency = 0;
    public float score = 0;
}