using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public struct S_StructRumbleData
{
    [Title("Channel")]
    public S_EnumRumbleChannel Channel;

    [Title("Frequency")]
    [Range(0, 1)] public float LowFrequency;   // 0..1 (motor “light”)

    [Range(0, 1)] public float HighFrequency;  // 0..1 (motor “heavy”)

    [Title("Time")]
    [SuffixLabel("s", Overlay = true)]
    public float Duration;       // en secondes

    [Title("Intensity")]
    public AnimationCurve CurveIntensityInTime; // curve intensity in the time of teh rumble 0 => 1

    [Title("Pause")]
    public bool UseUnscaledTime;
}
