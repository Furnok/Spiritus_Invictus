using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(S_SceneReference))]
public class S_SceneNameAttributeEditor : PropertyDrawer
{
    private static SceneAsset[] sceneAssets;
    private static string[] sceneNames;
    private static string[] lastScenePaths;

    static S_SceneNameAttributeEditor()
    {
        EditorApplication.update += CheckBuildSettings;
    }

    private static void CheckBuildSettings()
    {
        var currentPaths = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (lastScenePaths == null || !currentPaths.SequenceEqual(lastScenePaths))
        {
            lastScenePaths = currentPaths;
            RefreshScenes();

            var inspectors = Resources.FindObjectsOfTypeAll<Editor>().OfType<Editor>();
            foreach (var inspector in inspectors) inspector.Repaint();
        }
    }

    private static void RefreshScenes()
    {
        List<SceneAsset> scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path))
            .Where(s => s != null)
            .ToList();

        scenes.Insert(0, null);

        sceneAssets = scenes.ToArray();
        sceneNames = scenes.Select(s => s == null ? "None" : s.name).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (sceneAssets == null || sceneNames == null) RefreshScenes();

        // Get Properties
        SerializedProperty nameProp = property.FindPropertyRelative("sceneName");
        SerializedProperty guidProp = property.FindPropertyRelative("sceneGUID");
        SerializedProperty pathProp = property.FindPropertyRelative("scenePath");

        EditorGUI.BeginProperty(position, label, property);

        // Resolve Scene GUID
        string guid = guidProp.stringValue;
        string absolutePath = AssetDatabase.GUIDToAssetPath(guid);
        SceneAsset currentScene = string.IsNullOrEmpty(absolutePath) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(absolutePath);

        // Keep Name & Path Updated
        pathProp.stringValue = currentScene ? AssetDatabase.GetAssetPath(currentScene) : "";
        nameProp.stringValue = currentScene ? currentScene.name : "";

        // Find Current Index
        int currentIndex = Array.IndexOf(sceneAssets, currentScene);
        if (currentIndex < 0)
        {
            currentIndex = 0;

            // Apply Result
            nameProp.stringValue = "";
            guidProp.stringValue = "";
            pathProp.stringValue = "";
        }

        // Popup
        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, sceneNames);

        if (EditorGUI.EndChangeCheck())
        {
            // Apply Result
            if (newIndex == 0)
            {
                nameProp.stringValue = "";
                guidProp.stringValue = "";
                pathProp.stringValue = "";
            }
            else
            {
                SceneAsset scene = sceneAssets[newIndex];
                string path = AssetDatabase.GetAssetPath(scene);
                string newGUID = AssetDatabase.AssetPathToGUID(path);

                nameProp.stringValue = scene.name;
                guidProp.stringValue = newGUID;
                pathProp.stringValue = path;
            }
        }

        EditorGUI.EndProperty();
    }
}