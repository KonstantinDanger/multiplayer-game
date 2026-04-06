using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/Upgrade")]
public class ScriptableUpgrade : ScriptableObject
{
    [SerializeField] private UpgradeInfo _upgradeInfo;
    [SerializeReference, SubclassSelector] private Upgrade _upgrade;

    private void OnValidate()
    {
        string desc = ParseDescription(_upgradeInfo.RawDescription, _upgrade);
        _upgradeInfo.FormattedDescription = desc;
    }

    public Upgrade GetNew()
        => Utils.GetInstancedCopyOf(_upgrade);

    public UpgradeInfo GetInfo()
        => _upgradeInfo;

    private string ParseDescription(string rawText, object source)
    {
        if (string.IsNullOrEmpty(rawText) || source == null)
            return rawText;

        return Regex.Replace(rawText, @"\{(\w+)\}(?:\*\(([\d.-]+)\))?", m =>
        {
            string targetName = m.Groups[1].Value;
            string multiplierStr = m.Groups[2].Value;

            object result = FindMemberValue(source, targetName);

            if (result == null)
                return $"\"[{targetName}\" Not Found]";

            if (result is Single numResult && !String.IsNullOrEmpty(multiplierStr))
            {
                Single multiplier = Single.Parse(multiplierStr);

                return (numResult * multiplier).ToString();
            }

            return result.ToString();
        });
    }

    private object FindMemberValue(object obj, string name)
    {
        if (obj == null)
            return null;

        Type type = obj.GetType();

        PropertyInfo prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop != null)
            return prop.GetValue(obj);

        FieldInfo field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (field != null)
            return field.GetValue(obj);

        // Recursively search through nested structs and classes
        foreach (FieldInfo f in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (f.FieldType.IsPrimitive || f.FieldType == typeof(string) || f.FieldType.IsEnum)
                continue;

            object nestedValue = f.GetValue(obj);

            if (nestedValue == null)
                continue;

            object found = FindMemberValue(nestedValue, name);
            if (found != null)
                return found;
        }

        return null;
    }
}

