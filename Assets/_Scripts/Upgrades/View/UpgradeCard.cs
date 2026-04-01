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
    private Upgrade _upgrade;
    private event Action OnObtain;

    private void OnEnable()
        => _obtainButton.onClick.AddListener(Obtain);

    private void OnDisable()
        => _obtainButton.onClick.RemoveListener(Obtain);

    public void Initialize(GameObject owner, Upgrade upgrade, Action onObtain)
    {
        _owner = owner;
        _upgrade = upgrade;
        OnObtain = onObtain;
    }

    private void Obtain()
    {
        _upgrade.Perform(_owner);
        OnObtain?.Invoke();
    }
}
