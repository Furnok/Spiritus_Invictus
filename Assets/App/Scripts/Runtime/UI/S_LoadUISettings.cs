using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_LoadUISettings : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private S_Settings settings;
    [SerializeField] private TMP_Dropdown dropDownLanguages;
    [SerializeField] private Toggle toggleFullscreen;
    [SerializeField] private TMP_Dropdown dropDownResolutions;
    [SerializeField] private List<Slider> listSliderVolume;
    [SerializeField] private List<TextMeshProUGUI> listTextVolume;

    //[Header("Output")]
    //[SerializeField] private RSO_SettingsSaved rsoSettingsSaved;

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
        //dropDownLanguages.value = rsoSettingsSaved.Value.languageIndex;
    }

    private void LoadFullScreen()
    {
        //toggleFullscreen.isOn = rsoSettingsSaved.Value.fullScreen;
    }

    private int GetResolutions(int index)
    {
        List<Resolution> resolutionsPC = new(Screen.resolutions);
        resolutionsPC.Reverse();

        int currentResolutionIndex = 0;

        dropDownResolutions.ClearOptions();

        List<string> options = new();

        for (int i = 0; i < resolutionsPC.Count; i++)
        {
            Resolution res = resolutionsPC[i];
            string option = $"{res.width}x{res.height} {res.refreshRateRatio}Hz";

            options.Add(option);

            if (i == index)
            {
                currentResolutionIndex = i;
            }
        }

        dropDownResolutions.AddOptions(options);
        dropDownResolutions.RefreshShownValue();

        return currentResolutionIndex;
    }

    private void LoadResolutions()
    {
        //dropDownResolutions.value = GetResolutions(rsoSettingsSaved.Value.resolutionIndex);
    }

    private void LoadVolumes()
    {
        /*for (int i = 0; i < rsoSettingsSaved.Value.listVolumes.Count; i++)
        {
            listSliderVolume[i].value = rsoSettingsSaved.Value.listVolumes[i].volume;
            listTextVolume[i].text = $"{rsoSettingsSaved.Value.listVolumes[i].volume}%";
        }*/
    }
}