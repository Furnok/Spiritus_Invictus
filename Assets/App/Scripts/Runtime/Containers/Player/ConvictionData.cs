using UnityEngine;
using System;

[Serializable]
public struct ConvictionData
{
    public float maxConviction;
    public float healCost;
    public float ammountLostOverTick;
    public float tickIntervalSec;
    public float pauseIntervalAfterGained;
    public float pauseIntervalAfterLoss;
    public float dodgeSuccessfullGain;
    public float parrySuccesfullGain;
}
