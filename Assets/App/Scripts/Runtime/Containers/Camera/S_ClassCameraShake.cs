using Sirenix.OdinInspector;
using System;

[Serializable]
public class S_ClassCameraShake
{
    [Title("Shake")]
    public float amplitude = 0;
    public float frequency = 0;

    [Title("Time")]
    [SuffixLabel("s", Overlay = true)]
    public float duration = 0;
}