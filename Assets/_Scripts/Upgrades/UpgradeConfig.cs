using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade/UpgradeConfig")]
public class UpgradeConfig : ScriptableObject
{
    [field: SerializeField, Range(2, 5)] public int MaxUpgradesAtTime { get; private set; } = 3;
    [field: SerializeField, Range(2, 5)] public int GiveUpgradeEachLevel { get; private set; } = 3;
}