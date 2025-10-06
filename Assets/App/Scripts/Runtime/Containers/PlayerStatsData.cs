using System;
using UnityEngine;

[Serializable]
public struct PlayerStatsData
{
    public float maxHealth;
    public float maxConviction;
    public float healAmount;
    public float delayBeforeHeal;
    public float parryToleranceBeforeHit;
    public float parryToleranceAfterHit;
    public float parryDuration;
}
