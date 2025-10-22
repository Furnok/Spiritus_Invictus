using Sirenix.OdinInspector;
using System;
using UnityEngine.UI;

[Serializable]
public class S_ClassNavigation
{
    [Title("Default Selectable")]
    public Selectable selectableDefault;

    [Title("Selectable Focus")]
    public Selectable selectableFocus;

    [Title("Selectable Press Old Window")]
    public Selectable selectablePressOldWindow;

    [Title("Selectable Press Old")]
    public Selectable selectablePressOld;

    [Title("Selectable Press")]
    public Selectable selectablePress;
}