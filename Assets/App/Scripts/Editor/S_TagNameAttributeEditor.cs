using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(S_TagNameAttribute))]
public class S_TagNameAttributeEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use [TagName] with a String.");
        }
        else
        {
            string[] allTags = InternalEditorUtility.tags;

            List<string> namesWithNone = new List<string>();
            namesWithNone.Add("None");
            namesWithNone.AddRange(allTags);

            int selectedIndex = Array.IndexOf(namesWithNone.ToArray(), property.stringValue);
            if (selectedIndex < 0)
                selectedIndex = 0;

            int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, namesWithNone.ToArray());

            if (newIndex == 0)
            {
                property.stringValue = "";
            }
            else
            {
                property.stringValue = allTags[newIndex - 1];
            }
        }

        EditorGUI.EndProperty();
    }
}