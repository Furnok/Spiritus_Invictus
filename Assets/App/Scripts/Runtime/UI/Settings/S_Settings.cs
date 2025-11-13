using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class S_Settings : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Save")]
    [SerializeField, S_SaveName] private string saveSettingsName;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSaveData rseOnSaveData;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_SettingsSaved rsoSettingsSaved;

    private bool isLoaded = false;
    private List<TextMeshProUGUI> listTextAudios = new();

    private Bus audioMaster;
    private Bus audioMusic;
    private Bus audioSounds;
    private Bus audioUI;

    private void Awake()
    {
        audioMaster = RuntimeManager.GetBus("bus:/");
        audioMusic = RuntimeManager.GetBus("bus:/Music");
        audioSounds = RuntimeManager.GetBus("bus:/Sounds");
        audioUI = RuntimeManager.GetBus("bus:/UI");
    }

    private void OnEnable()
    {
        isLoaded = false;
    }

    public void Setup(List<TextMeshProUGUI> listTextVolumes)
    {
        isLoaded = true;

        listTextAudios = listTextVolumes;
    }

    public void UpdateLanguages(int index)
    {
        if (isLoaded && rsoSettingsSaved.Value.languageIndex != index)
        {
            rsoSettingsSaved.Value.languageIndex = index;
        }
    }

    public void UpdateHoldLockTarget(bool value)
    {
        if (isLoaded && rsoSettingsSaved.Value.holdLockTarget != value)
        {
            rsoSettingsSaved.Value.holdLockTarget = value;
        }
    }

    private Resolution GetResolutions(int index)
    {
        List<Resolution> resolutionsPC = new(Screen.resolutions);

        resolutionsPC = resolutionsPC
            .Where(r => r.width >= 1280 && r.height >= 720)
            .OrderByDescending(r => r.width * r.height)
            .ThenByDescending(r => r.refreshRateRatio.value)
            .ToList();

        Resolution resolution = resolutionsPC[0];

        for (int i = 0; i < resolutionsPC.Count; i++)
        {
            Resolution res = resolutionsPC[i];

            if (i == index)
            {
                resolution = res;
            }
        }

        return resolution;
    }

    public void UpdateResolutions(int index)
    {
        if (isLoaded && rsoSettingsSaved.Value.resolutionIndex != index)
        {
            rsoSettingsSaved.Value.resolutionIndex = index;
        }
    }

    public void UpdateFullscreen(bool value)
    {
        if (isLoaded && rsoSettingsSaved.Value.fullScreen != value)
        {
            rsoSettingsSaved.Value.fullScreen = value;
        }
    }

    public void UpdateMainVolume(float value)
    {
        if (isLoaded && rsoSettingsSaved.Value.listVolumes[0].volume != value)
        {
            rsoSettingsSaved.Value.listVolumes[0].volume = value;

            listTextAudios[0].text = $"{value}%";
        }
    }

    public void UpdateMusicVolume(float value)
    {
        if (isLoaded && rsoSettingsSaved.Value.listVolumes[1].volume != value)
        {
            rsoSettingsSaved.Value.listVolumes[1].volume = value;

            listTextAudios[1].text = $"{value}%";
        }
    }

    public void UpdateSoundsVolume(float value)
    {
        if (isLoaded && rsoSettingsSaved.Value.listVolumes[2].volume != value)
        {
            rsoSettingsSaved.Value.listVolumes[2].volume = value;

            listTextAudios[2].text = $"{value}%";
        }
    }

    public void UpdateUIVolume(float value)
    {
        if (isLoaded && rsoSettingsSaved.Value.listVolumes[3].volume != value)
        {
            rsoSettingsSaved.Value.listVolumes[3].volume = value;

            listTextAudios[3].text = $"{value}%";
        }
    }

    public void SaveSettings()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[rsoSettingsSaved.Value.languageIndex];

        Resolution resolution = GetResolutions(rsoSettingsSaved.Value.resolutionIndex);

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);

        if (rsoSettingsSaved.Value.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

        Screen.fullScreen = rsoSettingsSaved.Value.fullScreen;

        audioMaster.setVolume(rsoSettingsSaved.Value.listVolumes[0].volume / 100);
        audioMusic.setVolume(rsoSettingsSaved.Value.listVolumes[1].volume / 100);
        audioSounds.setVolume(rsoSettingsSaved.Value.listVolumes[2].volume / 100);
        audioUI.setVolume(rsoSettingsSaved.Value.listVolumes[3].volume / 100);

        rseOnSaveData.Call(saveSettingsName, true);
    }
}