using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public static class S_ProjectInitializer
{
    private const string RootFolder = "App";

    private static readonly string[] SubFolders = new[]
    {
        "Animations",
        "Arts",
        Path.Combine("Arts", "Sprites"),
        "Audio",
        Path.Combine("Audio", "Musics"),
        Path.Combine("Audio", "SFX"),
        Path.Combine("Audio", "UI"),
        "Inputs",
        "Prefabs",
        Path.Combine("Prefabs", "Managers"),
        Path.Combine("Prefabs", "UI"),
        "Scenes",
        Path.Combine("Scenes", "Tests"),
        "Scripts",
        Path.Combine("Scripts", "Editor"),
        Path.Combine("Scripts", "Runtime"),
        Path.Combine("Scripts", "Runtime", "Containers"),
        Path.Combine("Scripts", "Runtime", "Inputs"),
        Path.Combine("Scripts", "Runtime", "Managers"),
        Path.Combine("Scripts", "Runtime", "UI"),
        Path.Combine("Scripts", "Runtime", "Utils"),
        Path.Combine("Scripts", "Runtime", "Wrapper"),
        Path.Combine("Scripts", "Runtime", "Wrapper", "RSE"),
        Path.Combine("Scripts", "Runtime", "Wrapper", "RSO"),
        Path.Combine("Scripts", "Runtime", "Wrapper", "SSO"),
        "SOD",
        Path.Combine("SOD", "RSE"),
        Path.Combine("SOD", "RSO"),
        Path.Combine("SOD", "SSO"),
        "Plugins",
        "Resources",
        "ScriptTemplates",
        "Settings"
     };

    [MenuItem("Tools/Initialize Project Folders")]
    public static void CreateProjectFolders()
    {
        foreach (string folder in SubFolders.Prepend(RootFolder))
        {
            string fullPath = Path.Combine(Application.dataPath, folder);
            try
            {
                Directory.CreateDirectory(fullPath);
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to create folder '{fullPath}': {e.Message}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Project folder structure initialized.");
    }
}