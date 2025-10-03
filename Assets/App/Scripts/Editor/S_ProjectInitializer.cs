using UnityEditor;
using UnityEngine;
using System.IO;

//[InitializeOnLoad]
public static class S_ProjectInitializer
{
    static S_ProjectInitializer()
    {
        string rootFolder = $"App";

        // List of Subfolders to Create
        string[] folders = new string[]
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

            $"Plugins",

            $"Resources",

            $"ScriptTemplates",

            $"Settings",
        };

		// Check if the Root Folder Already Exists; if not, Create All the Subfolders
		string rootPath = Path.Combine(Application.dataPath, rootFolder);

        if (!Directory.Exists(rootPath))
        {
			Directory.CreateDirectory(rootPath);

			foreach (var folder in folders)
			{
				string folderPath = Path.Combine(Application.dataPath, folder);

				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}
			}

			// Refresh Unity AssetDatabase 
			AssetDatabase.Refresh();
		}
    }
}
