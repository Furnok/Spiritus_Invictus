using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(S_SaveNameAttribute))]
public class S_SaveNameAttributeEditor : PropertyDrawer
{
    private static readonly string[] saveNames;

    private static readonly bool haveSettings = true;
    private static readonly bool haveSaves = true;
    private static readonly int saveMax = 1;

    static S_SaveNameAttributeEditor()
    {
        List<string> names = new List<string>(saveMax + 2) { "None" };

        if (haveSettings)
        {
            names.Add("Settings");
        }

        if (haveSaves)
        {
            if (saveMax == 1)
            {
                names.Add($"Save");
            }
            else
            {
                for (int i = 1; i <= saveMax; i++)
                {
                    names.Add($"Save_{i}");
                }
            }
        }

        saveNames = names.ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // String Field Check
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use [SaveName] with a string field.");
            EditorGUI.EndProperty();
            return;
        }

        // Saves Disabled Check
        if (!haveSettings && !haveSaves)
        {
            EditorGUI.LabelField(position, label.text, "Saves Disabled");
            EditorGUI.EndProperty();
            return;
        }

        // Find Current Index
        int selectedIndex = Array.IndexOf(saveNames, property.stringValue);
        if (selectedIndex < 0) selectedIndex = 0;

        // Popup
        int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, saveNames);

        // Apply Result
        property.stringValue = (newIndex == 0) ? string.Empty : saveNames[newIndex];

        EditorGUI.EndProperty();
    }
}