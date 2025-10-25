using UnityEngine;
using System;

[Serializable]
public struct AnimationTransitionDelays
{
    [Header("Startup delay")]
    public float dodgeStartupDelay;
    public float parryStartupDelay;
    public float attackStartupDelay;
    public float healStartupDelay;

    [Header("Recovery delay")]
    public float dodgeRecoveryDelay;
    public float parryRecoveryDelay;
    public float attackRecoveryDelay;
    public float healRecoveryDelay;

}
