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

    private Upgrade _upgrade;

    private void OnEnable()
        => _obtainButton.onClick.AddListener(Obtain);

    private void OnDisable()
        => _obtainButton.onClick.RemoveListener(Obtain);

    public void Initialize(Upgrade upgrade)
        => _upgrade = upgrade;

    private void Obtain()
        => _upgrade.Perform(null); //Change to valid target
}
