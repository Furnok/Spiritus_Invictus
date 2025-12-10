using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(S_LayerNameAttribute))]
public class S_LayerNameAttributeEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Int Field Check
        if (property.propertyType != SerializedPropertyType.Integer)
        {
            EditorGUI.LabelField(position, label.text, "Use [LayerName] with a Int Field.");
        }
        else
        {
            // Popup & Apply Result
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }

        EditorGUI.EndProperty();
    }
}