using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(S_SceneReference))]
public class S_SceneNameAttributeEditor : PropertyDrawer
{
    private static SceneAsset[] cachedScenes;
    private static string[] cachedSceneNames;

    private static void CacheBuildScenes()
    {
        List<SceneAsset> scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path))
            .Where(s => s != null)
            .ToList();

        scenes.Insert(0, null);

        cachedScenes = scenes.ToArray();
        cachedSceneNames = scenes.Select(s => s == null ? "None" : s.name).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty nameProp = property.FindPropertyRelative("sceneName");
        SerializedProperty guidProp = property.FindPropertyRelative("sceneGUID");
        SerializedProperty pathProp = property.FindPropertyRelative("scenePath");

        EditorGUI.BeginProperty(position, label, property);

        CacheBuildScenes();

        string guid = guidProp.stringValue;
        string resolvedPath = AssetDatabase.GUIDToAssetPath(guid);
        SceneAsset currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(resolvedPath);

        pathProp.stringValue = resolvedPath;

        int currentIndex = Array.IndexOf(cachedScenes, currentScene);

        if (currentIndex < 0) 
        {
            currentIndex = 0;
        }

        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, cachedSceneNames);

        if (EditorGUI.EndChangeCheck())
        {
            if (newIndex >= 0 && newIndex < cachedScenes.Length)
            {
                SceneAsset selectedScene = cachedScenes[newIndex];
                string path = AssetDatabase.GetAssetPath(selectedScene);
                string newGUID = AssetDatabase.AssetPathToGUID(path);

                nameProp.stringValue = selectedScene != null ? selectedScene.name : "";
                guidProp.stringValue = newGUID;
                pathProp.stringValue = path;
            }
            else if (newIndex == 0)
            {
                nameProp.stringValue = "";
                guidProp.stringValue = "";
                pathProp.stringValue = "";
            }
        }

        EditorGUI.EndProperty();
    }
}