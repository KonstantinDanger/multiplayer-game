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

        string keyName = GetCurrentDeviceUpgradeKey();
        string[] strings = _keyToPressText.Split(new[] { '{', '}' }, System.StringSplitOptions.RemoveEmptyEntries);
        string str = "";

        foreach (string s in strings)
        {
            if (s == "key")
            {
                str += $" {keyName}";
                continue;
            }

            str += $" {s}";
        }

        return str;
    }

    private string GetCurrentDeviceUpgradeKey()
    {
        string readable = InputControlPath.ToHumanReadableString(
            _keyReference.action.bindings[0].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        return readable;
    }
}
