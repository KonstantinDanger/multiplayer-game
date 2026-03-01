using UnityEngine;
using UnityEngine.UI;

public class UpgradeView : MonoBehaviour
{
    [SerializeField] private Button _obtainButton;

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
