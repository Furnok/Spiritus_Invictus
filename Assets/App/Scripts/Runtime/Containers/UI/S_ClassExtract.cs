using Sirenix.OdinInspector;
using System;
using UnityEngine.Localization;

[Serializable]
public class S_ClassExtract
{
    [Title("Dialogue")]
    public LocalizedString text = null;

    [Title("Time")]
    [SuffixLabel("s", Overlay = true)]
    public int duration = 0;
}