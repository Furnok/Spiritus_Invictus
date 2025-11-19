using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(S_SaveNameAttribute))]
public class S_SaveNameAttributeEditor : PropertyDrawer
{
    private static List<string> cachedSaveNames = new();
    private static string[] cachedSaveNamesArray;

    private static readonly bool haveSettings = true;
    private static readonly bool haveSaves = true;
    private static readonly int saveMax = 1;

    private static void CacheSaveNames()
    {
        cachedSaveNames.Clear();

        cachedSaveNames.Add("None");

        if (haveSettings)
        {
            cachedSaveNames.Add("Settings");
        }
            
        if (haveSaves)
        {
            if (saveMax == 1)
            {
                cachedSaveNames.Add("Save");
            }
            else
            {
                for (int i = 0; i < saveMax; i++)
                    cachedSaveNames.Add($"Save_{i + 1}");
            }
        }

        cachedSaveNamesArray = cachedSaveNames.ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use [SaveName] with a String.");
            EditorGUI.EndProperty();
            return;
        }

        CacheSaveNames();

        if (!haveSettings && !haveSaves)
        {
            EditorGUI.LabelField(position, label.text, "Saves Disabled");
            EditorGUI.EndProperty();
            return;
        }

        int selectedIndex = Array.IndexOf(cachedSaveNamesArray, property.stringValue);

        if (selectedIndex < 0)
        {
            selectedIndex = 0;
        }

        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, cachedSaveNamesArray);
        if (EditorGUI.EndChangeCheck())
        {
            property.stringValue = cachedSaveNamesArray[newIndex];
        }

        EditorGUI.EndProperty();
    }
}