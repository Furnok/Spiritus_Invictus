using System;
using System.Collections.Generic;

[Serializable]
public class S_SettingsSaved
{
    public int languageIndex = 0;
    public bool fullScreen = true;
    public int resolutionIndex = 0;

    public List<S_ClassVolume> listVolumes = new()
    {
        new S_ClassVolume { name = "Master", volume = 100f },
        new S_ClassVolume { name = "Music", volume = 100f },
        new S_ClassVolume { name = "Sounds", volume = 100f },
        new S_ClassVolume { name = "UI", volume = 100f }
    };
}