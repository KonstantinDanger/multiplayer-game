using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchProgressHUD : HUD
{
    [SerializeField] private Slider _progressGauge;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _deathmatchText;

    private bool _deathmatchStarted;

    private void OnDisable()
    {
        _deathmatchStarted = false;
        _timerText.gameObject.SetActive(true);
        _timerText.text = string.Empty;
        _deathmatchText.gameObject.SetActive(false);
    }

    public void UpdateProgress(float matchTime, float normalizedMatchProgress, bool isDeathmatchStarted)
    {
        if (isDeathmatchStarted)
        {
            HandleDeathmatchStarted();
            return;
        }

        string formattedTime = Utils.ConvertSecondsToTimerFormat(matchTime);

        _timerText.text = formattedTime;

        _progressGauge.value = normalizedMatchProgress;
    }

    private void HandleDeathmatchStarted()
    {
        if (_deathmatchStarted)
            return;

        _timerText.gameObject.SetActive(false);
        _deathmatchText.gameObject.SetActive(true);

        _deathmatchStarted = true;
    }
}
