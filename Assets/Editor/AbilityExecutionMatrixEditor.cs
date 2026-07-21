using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(ParallelAbilityExecutionMatrix))]
public class AbilityExecutionMatrixEditor : Editor
{
    const float RowLabelWidth = 220f;
    const float CellWidth = 110f;
    const float CellHeight = 24f;
    const float HeaderHeight = 44f;

    Vector2 matrixScroll;

    SerializedProperty abilitiesProp;
    SerializedProperty transitionsProp;
    SerializedProperty valuesProp;

    private void OnEnable()
    {
        abilitiesProp = serializedObject.FindProperty("_abilities");
        transitionsProp = serializedObject.FindProperty("_transitions");
        valuesProp = transitionsProp.FindPropertyRelative("values");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(abilitiesProp, true);

        int count = abilitiesProp.arraySize;

        if (valuesProp.arraySize != count * count)
            valuesProp.arraySize = count * count;

        if (count > 0)
        {
            GUILayout.Space(10);

            DrawMatrix(count);
        }

        serializedObject.ApplyModifiedProperties();
    }

    void DrawMatrix(int count)
    {
        EditorGUILayout.LabelField("Parallel Ability Execution Matrix", EditorStyles.boldLabel);

        float matrixWidth = RowLabelWidth + count * CellWidth;
        float matrixHeight = HeaderHeight + count * CellHeight;

        matrixScroll = EditorGUILayout.BeginScrollView(
            matrixScroll,
            true,
            false,
            GUILayout.Height(matrixHeight + 20));

        Rect rect = GUILayoutUtility.GetRect(matrixWidth, matrixHeight);

        DrawMatrixContents(rect, count);

        EditorGUILayout.EndScrollView();
    }

    private void DrawMatrixContents(Rect rect, int count)
    {
        for (int c = 0; c < count; c++)
        {
            Object obj =
                abilitiesProp.GetArrayElementAtIndex(c).objectReferenceValue;

            string name = obj ? obj.name : "-";

            Rect header = new Rect(
                rect.x + RowLabelWidth + c * CellWidth,
                rect.y,
                CellWidth,
                HeaderHeight);

            GUI.Box(header, "");

            GUIStyle style = new GUIStyle(EditorStyles.miniLabel);
            style.alignment = TextAnchor.MiddleCenter;
            style.wordWrap = true;

            GUI.Label(header, name, style);
        }

        for (int r = 0; r < count; r++)
        {
            Object obj =
                abilitiesProp.GetArrayElementAtIndex(r).objectReferenceValue;

            string rowName = obj ? obj.name : "-";

            Rect rowHeader = new Rect(
                rect.x,
                rect.y + HeaderHeight + r * CellHeight,
                RowLabelWidth,
                CellHeight);

            GUI.Box(rowHeader, rowName);

            for (int c = 0; c < count; c++)
            {
                Rect cell = new Rect(
                    rect.x + RowLabelWidth + c * CellWidth,
                    rect.y + HeaderHeight + r * CellHeight,
                    CellWidth,
                    CellHeight);

                GUI.Box(cell, "");

                if (r == c)
                {
                    GUI.Label(
                        cell,
                        "—",
                        new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                        {
                            alignment = TextAnchor.MiddleCenter
                        });

                    continue;
                }

                int index = r * count + c;

                SerializedProperty value =
                    valuesProp.GetArrayElementAtIndex(index);

                Rect toggleRect = new Rect(
                    cell.x + (CellWidth - 18) * .5f,
                    cell.y + 2,
                    18,
                    18);

                value.boolValue = EditorGUI.Toggle(toggleRect, value.boolValue);
            }
        }
    }
}