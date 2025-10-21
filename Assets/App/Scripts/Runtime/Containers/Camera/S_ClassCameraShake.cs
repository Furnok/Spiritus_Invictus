using Sirenix.OdinInspector;
using System;

[Serializable]
public class S_ClassCameraShake
{
    [Title("Shake")]
    public float amplitude;
    public float frequency;

    [Title("Time")]
    public float duration;
}