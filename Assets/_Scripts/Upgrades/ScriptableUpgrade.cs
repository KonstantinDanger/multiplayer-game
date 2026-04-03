using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/Upgrade")]
public class ScriptableUpgrade : ScriptableObject
{
    [SerializeField] private UpgradeInfo _upgradeInfo;
    [SerializeReference, SubclassSelector] private Upgrade _upgrade;

    public Upgrade GetNew()
        => Utils.GetInstancedCopyOf(_upgrade);

    public UpgradeInfo GetInfo()
        => _upgradeInfo;
}

