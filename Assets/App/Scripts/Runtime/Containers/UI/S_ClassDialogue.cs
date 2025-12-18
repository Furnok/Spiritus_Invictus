using Sirenix.OdinInspector;
using System;

[Serializable]
public class S_ClassDialogue
{
    [Title("Dialogue")]
    public string text = "";

    [Title("Time")]
    [SuffixLabel("s", Overlay = true)]
    public float duration = 0;
}