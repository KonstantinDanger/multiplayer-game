using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugText : MonoBehaviour
{
    [SerializeField] private GameObject _rootCanvas;
    [SerializeField] private TextMeshProUGUI _debugText;

    private void Awake() => DontDestroyOnLoad(_rootCanvas);

    private void OnEnable() => Application.logMessageReceived += HandleMessageReceived;

    private void OnDisable() => Application.logMessageReceived -= HandleMessageReceived;

    private void HandleMessageReceived(string condition, string stackTrace, LogType type) => _debugText.text = $"{stackTrace} | Current scene {SceneManager.GetActiveScene().name}";
}
