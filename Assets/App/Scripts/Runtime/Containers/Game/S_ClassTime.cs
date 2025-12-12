using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class S_ClassTime
{
    [Title("General")]
    [Range(0f, 1f)] public float _slowScaleDodge = 0;

    [SuffixLabel("s", Overlay = true)]
    public float _hitStopDodge = 0;

    [SuffixLabel("s", Overlay = true)]
    public float _slowDurationDodge = 0;

    [SuffixLabel("s", Overlay = true)]
    public float _blendOutDodge = 0;

    [Title("Parry Configuration")]
    [SuffixLabel("s", Overlay = true)]
    public float _hitStopParry = 0;
}