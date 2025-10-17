using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;

public class S_DataManagement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, S_SaveName] private string saveSettingsName;
    [SerializeField, S_SaveName] private List<string> saveNames;

    [Header("References")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Input")]
    [SerializeField] private RSE_OnLoadData rseLoadData;
    [SerializeField] private RSE_OnSaveData rseSaveData;
    [SerializeField] private RSE_OnDeleteData rseDeleteData;

    [Header("Output")]
    [SerializeField] private RSO_SettingsSaved rsoSettingsSaved;
    [SerializeField] private RSO_ContentSaved rsoContentSaved;

    private static readonly string EncryptionKey = "ajekoBnPxI9jGbnYCOyvE9alNy9mM/Kw";
    private static readonly string SaveDirectory = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Saves");
    private static readonly bool fileCrypted = true;

    private void Awake()
    {
        rsoSettingsSaved.Value = new();
        rsoContentSaved.Value = new();
    }

    private void OnEnable()
    {
        rseSaveData.action += SaveToJson;
        rseLoadData.action += LoadFromJson;
        rseDeleteData.action += DeleteData;
    }

    private void OnDisable()
    {
        rseSaveData.action -= SaveToJson;
        rseLoadData.action -= LoadFromJson;
        rseDeleteData.action -= DeleteData;

        rsoSettingsSaved.Value = null;
        rsoContentSaved.Value = null;
    }

    private void Start()
    {
        Directory.CreateDirectory(SaveDirectory);

        if (saveSettingsName != null)
        {
            if (FileAlreadyExist(saveSettingsName))
            {
                LoadFromJson(saveSettingsName, true);
            }
            else
            {
                SaveToJson(saveSettingsName, true);
            }
        }
    }

    private static string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
        aes.GenerateIV();

        using MemoryStream memoryStream = new();
        memoryStream.Write(aes.IV, 0, aes.IV.Length);

        using (CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        using (StreamWriter writer = new(cryptoStream))
            writer.Write(plainText);

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    private static string Decrypt(string cipherText)
    {
        try
        {
            byte[] buffer = Convert.FromBase64String(cipherText);

            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
            aes.IV = buffer[..(aes.BlockSize / 8)];

            using MemoryStream memoryStream = new(buffer[(aes.BlockSize / 8)..]);
            using CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(cryptoStream);

            return reader.ReadToEnd();
        }
        catch
        {
            throw;
        }
    }

    private string GetFilePath(string name)
    {
        return Path.Combine(SaveDirectory, $"{name}.json");
    }

    private bool FileAlreadyExist(string name)
    {
        return File.Exists(GetFilePath(name));
    }

    private void SaveToJson(string name, bool isSetting)
    {
        if (name == null) return;

        string filePath = GetFilePath(name);

        string dataToSave = "";

        if (isSetting)
        {
            dataToSave = JsonUtility.ToJson(rsoSettingsSaved.Value);
        }
        else
        {
            dataToSave = JsonUtility.ToJson(rsoContentSaved.Value);
        }

        File.WriteAllText(filePath, fileCrypted ? Encrypt(dataToSave) : dataToSave);
    }

    private void LoadFromJson(string name, bool isSettings)
    {
        if (!FileAlreadyExist(name)) return;

        string filePath = GetFilePath(name);
        string jsonContent;

        try
        {
            jsonContent = File.ReadAllText(filePath);

            if (fileCrypted)
            {
                jsonContent = Decrypt(jsonContent);
            }

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                throw new Exception();
            }    
        }
        catch
        {
            SaveToJson(name, isSettings);
            return;
        }

        try
        {
            if (isSettings)
            {
                rsoSettingsSaved.Value = JsonUtility.FromJson<S_SettingsSaved>(jsonContent);

                if (rsoSettingsSaved.Value == null)
                {
                    throw new Exception();
                }

                StartCoroutine(S_Utils.DelayFrame(() => LoadSettings()));
            }
            else
            {
                rsoContentSaved.Value = JsonUtility.FromJson<S_ContentSaved>(jsonContent);

                if (rsoContentSaved.Value == null)
                {
                    throw new Exception();
                }
            }
        }
        catch
        {
            if (isSettings)
            {
                SaveToJson(name, isSettings);
            }
        }
    }

    private Resolution GetResolutions(int index)
    {
        List<Resolution> resolutionsPC = new(Screen.resolutions);
        resolutionsPC.Reverse();

        Resolution resolution = resolutionsPC[0];
        Resolution recommended = Screen.currentResolution;

        for (int i = 0; i < resolutionsPC.Count; i++)
        {
            Resolution res = resolutionsPC[i];

            if (index < 0)
            {
                if (res.width == recommended.width && res.height == recommended.height && Mathf.Approximately((float)res.refreshRateRatio.value, (float)recommended.refreshRateRatio.value))
                {
                    resolution = res;
                }
            }
            else if (i == index)
            {
                resolution = res;
            }
        }

        return resolution;
    }

    private void LoadSettings()
    {
        StartCoroutine(LangueSetup());

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

        for (int i = 0; i < rsoSettingsSaved.Value.listVolumes.Count; i++)
        {
            audioMixer.SetFloat(rsoSettingsSaved.Value.listVolumes[i].name, 40 * Mathf.Log10(Mathf.Max(rsoSettingsSaved.Value.listVolumes[i].volume, 1) / 100));
        }
    }

    private IEnumerator LangueSetup()
    {
        var initOperation = LocalizationSettings.InitializationOperation;
        yield return initOperation;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[rsoSettingsSaved.Value.languageIndex];
    }

    private void DeleteData(string name)
    {
        if (FileAlreadyExist(name))
        {
            string filePath = GetFilePath(name);

            File.Delete(filePath);
        }
    }
}
