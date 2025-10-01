using UnityEngine;
using System;

[Serializable]
public struct PlayerStatsData
{
    public float maxHealth;
    public float maxConviction;
    public float healAmount;
    public float delayBeforeHeal;
    //public float healthRegenFrequency;
    //[Range(0f, 100f)] public float totalPercentageHealthRegenByStack;
    //[Range(0f, 100f)] public float healthRegenPercentageRateOfTotal;
}
