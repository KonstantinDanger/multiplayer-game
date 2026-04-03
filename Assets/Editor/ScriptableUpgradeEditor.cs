using System;
using System.Reflection;
using System.Text.RegularExpressions;
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
        Upgrade upgrade = script.GetNew();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("RawDescription Preview:", EditorStyles.boldLabel);

        string parsedText = ParseDescription(info.RawDescription, upgrade);

        EditorGUILayout.HelpBox(parsedText, MessageType.Info);

        if (EditorGUI.EndChangeCheck())
        {
            info.FormattedDescription = parsedText;

            EditorUtility.SetDirty(script);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private string ParseDescription(string rawText, object source)
    {
        if (string.IsNullOrEmpty(rawText) || source == null)
            return rawText;

        return Regex.Replace(rawText, @"\{(\w+)\}", m =>
        {
            string targetName = m.Groups[1].Value;
            string result = FindMemberValue(source, targetName);

            return result ?? $"<color=red>[{targetName} Not Found]</color>";
        });
    }

    private string FindMemberValue(object obj, string name)
    {
        if (obj == null)
            return null;

        Type type = obj.GetType();

        PropertyInfo prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop != null)
            return prop.GetValue(obj)?.ToString();

        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (field != null)
            return field.GetValue(obj)?.ToString();

        // Recursively search through nested structs and classes
        foreach (FieldInfo f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (f.FieldType.IsPrimitive || f.FieldType == typeof(string) || f.FieldType.IsEnum)
                continue;

            object nestedValue = f.GetValue(obj);

            if (nestedValue == null)
                continue;

            string found = FindMemberValue(nestedValue, name);
            if (found != null)
                return found;
        }

        return null;
    }
}
