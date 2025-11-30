using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomPropertyDrawer(typeof(S_AnimationNameAttribute))]
public class S_AnimationNameAttributeEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // String Field Check
        if (property.propertyType != SerializedPropertyType.String)
        {
            DrawError(position, label, "Use [S_AnimationName] with a string field.");
            return;
        }

        // MonoBehaviour Check
        if (!(property.serializedObject.targetObject is MonoBehaviour mb))
        {
            DrawError(position, label, "Target must be a MonoBehaviour.");
            return;
        }

        // Animator Field Check
        Animator animator = GetAnimatorFromField(property);

        // Animator GameObject Check
        if (animator == null)
        {
            animator = mb.GetComponent<Animator>();
        }
            
        if (animator == null)
        {
            DrawError(position, label, "No Animator found.");
            return;
        }

        // AnimatorController Check
        AnimatorController controller = GetAnimatorController(animator);
        if (controller == null)
        {
            DrawError(position, label, "Animator has no valid AnimatorController.");
            return;
        }

        // Parameters Check
        AnimatorControllerParameter[] parameters = controller.parameters;
        if (parameters == null || parameters.Length == 0)
        {
            DrawError(position, label, "Animator has no parameters.");
            return;
        }

        // Build List
        int count = parameters.Length;
        string[] options = new string[count + 1];
        options[0] = "None";

        for (int i = 0; i < count; i++)
        {
            options[i + 1] = parameters[i].name;
        }


        // Find Current Index
        string value = property.stringValue;
        int selectedIndex = 0;
        for (int i = 1; i < options.Length; i++)
        {
            if (options[i] == value)
            {
                selectedIndex = i;
                break;
            }
        }

        // Popup
        int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, options);

        // Apply Result
        property.stringValue = (newIndex == 0) ? string.Empty : options[newIndex];

        EditorGUI.EndProperty();
    }

    private Animator GetAnimatorFromField(SerializedProperty property)
    {
        var attr = (S_AnimationNameAttribute)attribute;
        if (string.IsNullOrEmpty(attr.animatorFieldName)) return null;

        SerializedProperty animatorProp = property.serializedObject.FindProperty(attr.animatorFieldName);

        if (animatorProp != null && animatorProp.propertyType == SerializedPropertyType.ObjectReference)
        {
            return animatorProp.objectReferenceValue as Animator;
        }

        return null;
    }

    private AnimatorController GetAnimatorController(Animator animator)
    {
        var rac = animator.runtimeAnimatorController;
        if (!rac) return null;

        if (rac is AnimatorOverrideController overrideCtrl)
        {
            return overrideCtrl.runtimeAnimatorController as AnimatorController;
        }

        return rac as AnimatorController;
    }

    private void DrawError(Rect position, GUIContent label, string message)
    {
        EditorGUI.LabelField(position, label.text, message);
        EditorGUI.EndProperty();
    }
}