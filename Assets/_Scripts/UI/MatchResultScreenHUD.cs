using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchResultScreenHUD : HUD
{
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Button _proceedButton;
    [SerializeField] private TextMeshProUGUI _countdownText;

    private void Awake()
        => _proceedButton.enabled = false;

    private void OnEnable()
        => _proceedButton.onClick.AddListener(HandleProceed);

    private void OnDisable()
        => _proceedButton.onClick.RemoveListener(HandleProceed);

    private void HandleProceed()
        => gameObject.SetActive(false);

    public void Initialize(MatchResult result, GameMatchData.Data matchSummaryData, float timeBeforeProceed)
    {
        gameObject.SetActive(true);

        switch (result)
        {
            case MatchResult.OneSided:
                _resultText.text = $"Player {matchSummaryData.winner} has won";
                break;

            case MatchResult.Draw:
                _resultText.text = $"Match ended in a draw";
                break;

            case MatchResult.CanceledEarly:
                _resultText.text = $"Match was canceled";
                break;

            case MatchResult.Surrender:
                _resultText.text = $"Player {matchSummaryData.loser} has surrendered" +
                    $"\nPlayer {matchSummaryData.winner} has won";
                break;
        }

        StartCoroutine(WaitBeforeProceedRoutine(timeBeforeProceed));
    }

    private IEnumerator WaitBeforeProceedRoutine(float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            int timeLeft = Mathf.CeilToInt(time - elapsedTime);
            _countdownText.text = $"{timeLeft}...";
            yield return null;
        }

        _countdownText.text = "";
        _proceedButton.enabled = true;
    }
}
