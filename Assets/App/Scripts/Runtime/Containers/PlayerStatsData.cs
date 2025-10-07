using System;
using UnityEngine;

[Serializable]
public struct PlayerStatsData
{
    [Header("Player values")]
    public float maxHealth;
    public float maxConviction;
    public float healAmount;
    public float delayBeforeHeal;

    [Header("Movement values")]


    [Header("Parry values")]
    public float parryToleranceBeforeHit;
    public float parryToleranceAfterHit;
    public float parryDuration;
}
