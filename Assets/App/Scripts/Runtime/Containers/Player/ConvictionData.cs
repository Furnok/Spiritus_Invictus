using UnityEngine;
using System;

[Serializable]
public struct ConvictionData
{
    public float maxConviction;
    public float startConviction;
    public float healCost;
    public float ammountLostOverTick;
    public float tickIntervalSec;
    public float pauseIntervalAfterGained;
    public float pauseIntervalAfterLoss;
    public float dodgeSuccessGain;
    public float parrySuccesGain;
}
