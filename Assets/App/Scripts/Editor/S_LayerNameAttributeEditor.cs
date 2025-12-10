using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(S_LayerNameAttribute))]
public class S_LayerNameAttributeEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use [LayerName] with Int Field.");
        }
    }
}