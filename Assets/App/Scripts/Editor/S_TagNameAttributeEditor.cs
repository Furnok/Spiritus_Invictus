using System;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(S_TagNameAttribute))]
public class S_TagNameAttributeEditor : PropertyDrawer
{
    private static string[] allTags;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use [TagName] with a String.");
        }
        else
        {
            // Get All Unity Tags
            if (allTags == null || allTags.Length == 0)
            {
                allTags = InternalEditorUtility.tags;
            }

            // Find the Index of the Current Tag
            int selectedIndex = Array.IndexOf(allTags, property.stringValue);

            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }

            property.stringValue = allTags[EditorGUI.Popup(position, label.text, selectedIndex, allTags)];
        }

        EditorGUI.EndProperty();
    }
}