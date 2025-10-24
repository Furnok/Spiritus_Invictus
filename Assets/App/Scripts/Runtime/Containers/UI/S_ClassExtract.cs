using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class S_ClassExtract
{
    [Title("Dialogue")]
    [TextArea(1, 20)]
    public string text;

    [Title("Time")]
    [SuffixLabel("s", Overlay = true)]
    public int duration;
}