using UnityEditor;
using UnityEngine;
using System.IO;

public static class S_ProjectInitializer
{
    [MenuItem("Tools/Initialize Project Folders")]
    public static void CreateProjectFolders()
    {
        string rootFolder = "App";

        string[] folders = new[]
        {
            rootFolder,
            $"{rootFolder}/Animations",
            $"{rootFolder}/Arts",
            $"{rootFolder}/Arts/Sprites",
            $"{rootFolder}/Audio",
            $"{rootFolder}/Audio/Musics",
            $"{rootFolder}/Audio/SFX",
            $"{rootFolder}/Audio/UI",
            $"{rootFolder}/Inputs",
            $"{rootFolder}/Prefabs",
            $"{rootFolder}/Prefabs/Managers",
            $"{rootFolder}/Prefabs/UI",
            $"{rootFolder}/Scenes",
            $"{rootFolder}/Scenes/Tests",
            $"{rootFolder}/Scripts",
            $"{rootFolder}/Scripts/Editor",
            $"{rootFolder}/Scripts/Runtime",
            $"{rootFolder}/Scripts/Runtime/Containers",
            $"{rootFolder}/Scripts/Runtime/Inputs",
            $"{rootFolder}/Scripts/Runtime/Managers",
            $"{rootFolder}/Scripts/Runtime/UI",
            $"{rootFolder}/Scripts/Runtime/Utils",
            $"{rootFolder}/Scripts/Runtime/Wrapper",
            $"{rootFolder}/Scripts/Runtime/Wrapper/RSE",
            $"{rootFolder}/Scripts/Runtime/Wrapper/RSO",
            $"{rootFolder}/Scripts/Runtime/Wrapper/SSO",
            $"{rootFolder}/SOD",
            $"{rootFolder}/SOD/RSE",
            $"{rootFolder}/SOD/RSO",
            $"{rootFolder}/SOD/SSO",
            "Plugins",
            "Resources",
            "ScriptTemplates",
            "Settings"
        };

        foreach (string folder in folders)
        {
            string path = Path.Combine(Application.dataPath, folder);
            Directory.CreateDirectory(path); // safe even if folder exists
        }

        AssetDatabase.Refresh();
        Debug.Log("Project folder structure initialized.");
    }
}