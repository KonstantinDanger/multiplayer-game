using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ScriptableCharacterClass))]
public class ScriptableCharacterClassEditor : Editor
{
    private SerializedProperty _abilitiesProp;
    private ReorderableList _abilitiesList;

    private void OnEnable()
    {
        _abilitiesProp = serializedObject.FindProperty("_abilities");

        _abilitiesList = new ReorderableList(serializedObject, _abilitiesProp, true, true, true, true);

        _abilitiesList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Abilities", EditorStyles.boldLabel);
        };

        _abilitiesList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var element = _abilitiesProp.GetArrayElementAtIndex(index);
            rect.y += 2;

            var abilityObj = element.objectReferenceValue as ScriptableAbility;
            string label = abilityObj != null ? abilityObj.name : "Empty";

            element.objectReferenceValue = EditorGUI.ObjectField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent($"Ability {index + 1}: {label}"),
                element.objectReferenceValue,
                typeof(ScriptableAbility),
                false
            );
        };

        _abilitiesList.elementHeightCallback = index =>
        {
            return EditorGUIUtility.singleLineHeight + 6;
        };

        _abilitiesList.onAddCallback = list =>
        {
            _abilitiesProp.arraySize++;
            var newElement = _abilitiesProp.GetArrayElementAtIndex(_abilitiesProp.arraySize - 1);
            newElement.objectReferenceValue = null;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("===|Character Class|===", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);
        _abilitiesList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
