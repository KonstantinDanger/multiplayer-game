using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _obtainButton;
    [SerializeField] private TextMeshProUGUI _upgradeName;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _statsDescription;

    private GameObject _owner;
    private UpgradeInfo _info;
    private Upgrade _upgrade;
    private event Action OnObtain;

    private void OnEnable()
        => _obtainButton.onClick.AddListener(Obtain);

    private void OnDisable()
        => _obtainButton.onClick.RemoveListener(Obtain);

    public void Initialize(GameObject owner, Upgrade upgrade, UpgradeInfo info, Action onObtain)
    {
        _owner = owner;
        _info = info;
        _upgrade = upgrade;
        OnObtain = onObtain;

        InitializeView();
    }

    private void InitializeView()
    {
        _icon.sprite = _info.Sprite;
        _upgradeName.text = _info.Name;
        _description.text = _info.FormattedDescription;
        _statsDescription.text = _info.StatsDescription;

        if (string.IsNullOrEmpty(_info.FormattedDescription))
            Debug.LogError("No formatted description available!");
    }

    private void Obtain()
    {
        _upgrade.Perform(_owner);
        OnObtain?.Invoke();
    }
}
