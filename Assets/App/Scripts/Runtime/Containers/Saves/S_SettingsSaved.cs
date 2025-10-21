using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[Serializable]
public class S_SettingsSaved
{
    [Title("Langues")]
    public int languageIndex = 0;

    [Title("Screen")]
    public bool fullScreen = true;
    public int resolutionIndex = -1;

    [Title("Audio")]
    public List<S_ClassVolume> listVolumes = new()
    {
        new S_ClassVolume { name = "Master", volume = 100f },
        new S_ClassVolume { name = "Music", volume = 100f },
        new S_ClassVolume { name = "Sounds", volume = 100f },
        new S_ClassVolume { name = "UI", volume = 100f }
    };
}