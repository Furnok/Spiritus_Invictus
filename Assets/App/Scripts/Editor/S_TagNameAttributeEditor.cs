using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(S_TagNameAttribute))]
public class S_TagNameAttributeEditor : PropertyDrawer
{
    private static readonly string[] tags;

    static S_TagNameAttributeEditor()
    {
        var allTags = InternalEditorUtility.tags;
        tags = new string[allTags.Length + 1];
        tags[0] = "None";
        Array.Copy(allTags, 0, tags, 1, allTags.Length);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // String Field Check
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use [TagName] with a string field.");
        }
        else
        {
            // Find Current Index
            int selectedIndex = Array.IndexOf(tags, property.stringValue);
            if (selectedIndex < 0) selectedIndex = 0;

            // Popup
            int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, tags);

            // Apply Result
            property.stringValue = (newIndex == 0) ? string.Empty : tags[newIndex];
        }

        EditorGUI.EndProperty();
    }
}