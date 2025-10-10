using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_LoadUISettings : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private S_Settings settings;
    [SerializeField] private TMP_Dropdown dropDownLanguages;
    [SerializeField] private TMP_Dropdown dropDownResolutions;
    [SerializeField] private Toggle toggleFullscreen;
    [SerializeField] private List<Slider> listSliderVolume;
    [SerializeField] private List<TextMeshProUGUI> listTextVolume;

    [Header("Output")]
    [SerializeField] private RSO_SettingsSaved rsoSettingsSaved;

    private void OnEnable()
    {
        LoadUI();
    }

    private void LoadUI()
    {
        LoadLanguages();

        LoadFullScreen();

        LoadResolutions();

        LoadVolumes();

        StartCoroutine(S_Utils.DelayFrame(() => settings.Setup(listTextVolume)));
    }

    private void LoadLanguages()
    {
        dropDownLanguages.value = rsoSettingsSaved.Value.languageIndex;
    }

    private void LoadFullScreen()
    {
        toggleFullscreen.isOn = rsoSettingsSaved.Value.fullScreen;
    }

    private int GetResolutions(int index)
    {
        List<Resolution> resolutionsPC = new(Screen.resolutions);

        resolutionsPC = resolutionsPC
            .Where(r => r.width >= 1280 && r.height >= 720)
            .OrderByDescending(r => r.width * r.height)
            .ThenByDescending(r => r.refreshRateRatio.value)
            .ToList();

        int currentResolutionIndex = 0;

        dropDownResolutions.GetComponent<TMP_Dropdown>().ClearOptions();

        List<string> options = new();

        for (int i = 0; i < resolutionsPC.Count; i++)
        {
            Resolution res = resolutionsPC[i];

            string option = $"{res.width}x{res.height} {res.refreshRateRatio.value:F2}Hz";

            options.Add(option);

            if (i == index)
            {
                currentResolutionIndex = options.Count - 1;
            }
        }

        dropDownResolutions.GetComponent<TMP_Dropdown>().AddOptions(options);
        dropDownResolutions.GetComponent<TMP_Dropdown>().RefreshShownValue();

        return currentResolutionIndex;
    }

    private void LoadResolutions()
    {
        dropDownResolutions.value = GetResolutions(rsoSettingsSaved.Value.resolutionIndex);
    }

    private void LoadVolumes()
    {
        for (int i = 0; i < rsoSettingsSaved.Value.listVolumes.Count; i++)
        {
            listSliderVolume[i].value = rsoSettingsSaved.Value.listVolumes[i].volume;
            listTextVolume[i].text = $"{rsoSettingsSaved.Value.listVolumes[i].volume}%";
        }
    }
}