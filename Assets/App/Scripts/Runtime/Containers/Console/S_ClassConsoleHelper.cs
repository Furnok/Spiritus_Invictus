using Sirenix.OdinInspector;
using System;
using UnityEngine.Localization;

[Serializable]
public class S_ClassConsoleHelper
{
    [Title("Command")]
    public string command = "";

    [Title("Description")]
    public LocalizedString description = null;
}