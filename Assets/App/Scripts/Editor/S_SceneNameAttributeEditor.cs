using System;
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
        cachedScenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path))
            .Where(s => s != null)
            .ToArray();

        cachedSceneNames = cachedScenes.Select(s => s.name).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty guidProp = property.FindPropertyRelative("sceneGUID");
        SerializedProperty pathProp = property.FindPropertyRelative("scenePath");

        EditorGUI.BeginProperty(position, label, property);

        CacheBuildScenes();

        // Get current scene from GUID
        string guid = guidProp.stringValue;
        string resolvedPath = AssetDatabase.GUIDToAssetPath(guid);
        SceneAsset currentScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(resolvedPath);

        // Keep scenePath in sync with GUID
        pathProp.stringValue = resolvedPath;

        // Show popup
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

                guidProp.stringValue = newGUID;
                pathProp.stringValue = path;
            }
        }

        EditorGUI.EndProperty();
    }
}