using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;

public class S_Settings : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("Save")]
    [SerializeField, S_SaveName] private string saveSettingsName;

    [TabGroup("References")]
    [Title("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [TabGroup("Outputs")]
    [SerializeField] private RSE_OnSaveData rseOnSaveData;

    [TabGroup("Outputs")]
    [SerializeField] private RSO_SettingsSaved rsoSettingsSaved;

    private bool isLoaded = false;
    private List<TextMeshProUGUI> listTextAudios = new();

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
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

            rsoSettingsSaved.Value.languageIndex = index;

            Save();
        }
    }

    public void UpdateHoldLockTarget(bool value)
    {
        if (isLoaded && rsoSettingsSaved.Value.holdLockTarget != value)
        {
            rsoSettingsSaved.Value.holdLockTarget = value;

            Save();
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
            Resolution resolution = GetResolutions(index);

            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);

            rsoSettingsSaved.Value.resolutionIndex = index;

            Save();
        }
    }

    public void UpdateFullscreen(bool value)
    {
        if (isLoaded && rsoSettingsSaved.Value.fullScreen != value)
        {
            if (value)
            {
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else
            {
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }

            Screen.fullScreen = value;

            rsoSettingsSaved.Value.fullScreen = value;

            Save();
        }
    }

    public void UpdateMainVolume(float value)
    {
        if (isLoaded && rsoSettingsSaved.Value.listVolumes[0].volume != value)
        {
            UpdateVolumes(value, 0);
        }
    }

    public void UpdateMusicVolume(float value)
    {
        if (isLoaded && rsoSettingsSaved.Value.listVolumes[1].volume != value)
        {
            UpdateVolumes(value, 1);
        }
    }

    public void UpdateSoundsVolume(float value)
    {
        if (isLoaded && rsoSettingsSaved.Value.listVolumes[2].volume != value)
        {
            UpdateVolumes(value, 2);
        }
    }

    public void UpdateUIVolume(float value)
    {
        if (isLoaded && rsoSettingsSaved.Value.listVolumes[3].volume != value)
        {
            UpdateVolumes(value, 3);
        }
    }

    private void UpdateVolumes(float value, int index)
    {
        audioMixer.SetFloat(rsoSettingsSaved.Value.listVolumes[index].name, 40 * Mathf.Log10(Mathf.Max(value, 1) / 100));

        rsoSettingsSaved.Value.listVolumes[index].volume = value;

        listTextAudios[index].text = $"{value}%";

        Save();
    }

    private void Save()
    {
        rseOnSaveData.Call(saveSettingsName, true);
    }
}