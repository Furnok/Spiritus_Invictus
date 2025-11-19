using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomPropertyDrawer(typeof(S_AnimationNameAttribute))]
public class S_AnimationNameAttributeEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use [S_AnimatorParameter] on a String.");
            EditorGUI.EndProperty();
            return;
        }

        if (!(property.serializedObject.targetObject is MonoBehaviour mb))
        {
            EditorGUI.LabelField(position, label.text, "Target is not a MonoBehaviour.");
            EditorGUI.EndProperty();
            return;
        }

        Animator animator = mb.GetComponent<Animator>();
        if (animator == null)
        {
            EditorGUI.LabelField(position, label.text, "No Animator Found on this GameObject.");
            EditorGUI.EndProperty();
            return;
        }

        RuntimeAnimatorController rac = animator.runtimeAnimatorController;
        AnimatorController controller = null;

        if (rac is AnimatorOverrideController overrideController)
        {
            controller = overrideController.runtimeAnimatorController as AnimatorController;
        }
        else
        {
            controller = rac as AnimatorController;
        }

        if (controller == null)
        {
            EditorGUI.LabelField(position, label.text, "No Valid AnimatorController Assigned.");
            EditorGUI.EndProperty();
            return;
        }

        var parameters = controller.parameters;

        if (parameters.Length == 0)
        {
            EditorGUI.LabelField(position, label.text, "No Parameters in AnimatorController.");
            EditorGUI.EndProperty();
            return;
        }

        string[] parameterNames = Array.ConvertAll(parameters, p => p.name);

        List<string> namesWithNone = new List<string>();
        namesWithNone.Add("None");
        namesWithNone.AddRange(parameterNames);

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
            property.stringValue = parameterNames[newIndex - 1];
        }

        EditorGUI.EndProperty();
    }
}