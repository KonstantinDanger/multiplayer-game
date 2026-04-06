using UnityEditor;

[CustomEditor(typeof(ScriptableUpgrade))]
public class ScriptableUpgradeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        ScriptableUpgrade script = (ScriptableUpgrade)target;
        UpgradeInfo info = script.GetInfo();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Live Preview:", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(info.FormattedDescription, MessageType.Info);


        serializedObject.ApplyModifiedProperties();
    }
}
