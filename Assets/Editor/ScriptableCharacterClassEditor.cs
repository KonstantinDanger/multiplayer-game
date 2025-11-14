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

            Color oldColor = GUI.color;

            if (index == 0)
            {
                Color color = new Color(1f, 0.85f, 0.6f);
                GUI.color = color;
                float thickness = 1f;

                Rect frameRect = new Rect(rect.x - 2, rect.y - 2, rect.width + 4, EditorGUIUtility.singleLineHeight + 6);
                EditorGUI.DrawRect(new Rect(frameRect.x, frameRect.y, frameRect.width, thickness), color);
                EditorGUI.DrawRect(new Rect(frameRect.x, frameRect.yMax - thickness, frameRect.width, thickness), color);
                EditorGUI.DrawRect(new Rect(frameRect.x, frameRect.y, thickness, frameRect.height), color);
                EditorGUI.DrawRect(new Rect(frameRect.xMax - thickness, frameRect.y, thickness, frameRect.height), color);

                GUI.color = new Color(1f, 0.84f, 0f);

            }

            var abilityObj = element.objectReferenceValue as ScriptableAbility;
            string label = abilityObj != null ? abilityObj.name : "Empty";
            string guiMessage = index == 0 ? "Primary ability" : $"Ability {index}: {label}";


            element.objectReferenceValue = EditorGUI.ObjectField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                new GUIContent(guiMessage),
                element.objectReferenceValue,
                typeof(ScriptableAbility),
                false
            );

            GUI.color = oldColor;

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
