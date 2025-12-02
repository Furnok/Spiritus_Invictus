using UnityEngine;
using System;

[Serializable]
public struct S_StructRumbleData
{
    public RumbleChannel Channel;
    public float LowFrequency;   // 0..1 (motor “light”)
    public float HighFrequency;  // 0..1 (motor “heavy”)
    public float Duration;       // en secondes
    public AnimationCurve CurveIntensityInTime; // curve intensity in the time of teh rumble 0 => 1
    public bool UseUnscaledTime;
}
