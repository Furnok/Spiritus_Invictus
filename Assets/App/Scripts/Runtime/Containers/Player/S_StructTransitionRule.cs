using Sirenix.OdinInspector;
using System;

[Serializable]
public struct S_StructTransitionRule
{
    [Title("Enum")]
    public S_EnumPlayerState forbiddenFrom;

    public S_EnumPlayerState forbiddenTo;
}
