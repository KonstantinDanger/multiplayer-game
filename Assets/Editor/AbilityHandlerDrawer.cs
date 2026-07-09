using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AbilitySlot))]
public class AbilityHandlerDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Get the serialized Ability field
        SerializedProperty abilityProp = property.FindPropertyRelative("<Ability>k__BackingField");

        // Only account for the height of the Ability field itself
        return EditorGUI.GetPropertyHeight(abilityProp, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the "Ability" field only
        SerializedProperty abilityProp = property.FindPropertyRelative("<Ability>k__BackingField");

        EditorGUI.PropertyField(position, abilityProp, new GUIContent(label.text), true);

        EditorGUI.EndProperty();
    }
}