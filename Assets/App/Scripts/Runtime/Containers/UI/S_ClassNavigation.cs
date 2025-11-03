using Sirenix.OdinInspector;
using System;
using UnityEngine.UI;

[Serializable]
public class S_ClassNavigation
{
    [Title("Default Selectable")]
    public Selectable selectableDefault = null;

    [Title("Selectable Focus")]
    public Selectable selectableFocus = null;

    [Title("Selectable Press Old Window")]
    public Selectable selectablePressOldWindow = null;

    [Title("Selectable Press Old")]
    public Selectable selectablePressOld = null;

    [Title("Selectable Press")]
    public Selectable selectablePress = null;
}