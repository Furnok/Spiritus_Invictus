using Sirenix.OdinInspector;
using System;

[Serializable]
public struct S_StructConvictionData
{
    [Title("Data")]
    public float maxConviction;

    public float startConviction;

    public float healCost;

    public float ammountLostOverTick;

    [SuffixLabel("s", Overlay = true)]
    public float tickIntervalSec;

    [SuffixLabel("s", Overlay = true)]
    public float pauseIntervalAfterGained;

    [SuffixLabel("s", Overlay = true)]
    public float pauseIntervalAfterLoss;

    public float dodgeSuccessGain;

    public float parrySuccesGain;
}