using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeNotifier : MonoBehaviour
{
    [Header("This string should contain keyword 'key' wrapped in following braces: '{}'. \nThis is required to parse the real reference of the button into that string")]
    [SerializeField] private string _keyToPressText;

    [SerializeField] private InputActionReference _keyReference;
    [SerializeField] private TextMeshProUGUI _keyToOpenUpgradeMenuText;

    private string _parsedText = "";

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(_parsedText))
            return;

        _parsedText = ParseText();
        _keyToOpenUpgradeMenuText.text = _parsedText;
    }

    private string ParseText()
    {
        if (string.IsNullOrEmpty(_keyToPressText))
            throw new System.Exception("_keyToPressText is empty!");

        string[] strings = _keyToPressText.Split(new[] { '{', '}' }, System.StringSplitOptions.RemoveEmptyEntries);
        string str = "";

        foreach (string s in strings)
        {
            if (s == "}" || s == "key")
            {
                continue;
            }

            if (s == "{")
            {
                str += $" {_keyReference.action.bindings[0].name}";
                continue;
            }

            str += $" {s}";
        }

        return str;
    }
}
