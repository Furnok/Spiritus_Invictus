using Sirenix.OdinInspector;
using System;

[Serializable]
public struct S_StructAnimationTransitionDelays
{
    [Title("Startup Delay")]
    [SuffixLabel("s", Overlay = true)]
    public float dodgeStartupDelay;

    [SuffixLabel("s", Overlay = true)]
    public float parryStartupDelay;

    [SuffixLabel("s", Overlay = true)]
    public float attackStartupDelay;

    [SuffixLabel("s", Overlay = true)]
    public float healStartupDelay;

    [Title("Recovery Delay")]
    [SuffixLabel("s", Overlay = true)]
    public float dodgeRecoveryDelay;

    [SuffixLabel("s", Overlay = true)]
    public float parryRecoveryDelay;

    [SuffixLabel("s", Overlay = true)]
    public float attackRecoveryDelay;

    [SuffixLabel("s", Overlay = true)]
    public float healRecoveryDelay;

}
