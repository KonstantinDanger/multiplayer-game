using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StaticData")]
public class StaticData : ScriptableObject
{
    [field: SerializeField] public string StartingSceneName { get; private set; } = "LobbyScene";
    [field: SerializeField] public string LobbySceneName { get; private set; } = "LobbyScene";
    [field: SerializeField] public string GameSceneName { get; private set; }
    [field: SerializeField] public GameMatchConfig GameMatchData { get; private set; }
    [field: SerializeField] public CharacterClassList ClassList { get; private set; }
    [field: SerializeField] public Player PlayerPrefab { get; private set; }
    [field: SerializeField] public CustomNetworkManager NetworkManagerPrefab { get; private set; }
    [field: SerializeField] public Zone ZonePrefab { get; internal set; }
    [field: SerializeField] public GameFactory GameFactoryPrefab { get; private set; }
    [field: SerializeField] public float ZoneDPS { get; private set; } = 10;
    [field: SerializeField] public DamageText DamageTextPrefab { get; private set; }

    //UI
    [field: SerializeField] public GameUI UIPrefab { get; private set; }
    [field: SerializeField] public LobbyView LobbyUIPrefab { get; private set; }
    [field: SerializeField] public PlayerInfoUI PlayerInfoUI { get; private set; }
    [field: SerializeField] public PlayerHUD PlayerHUDPrefab { get; private set; }
    [field: SerializeField] public AnimatorUpdaterConfig AnimatorUpdaterConfig { get; private set; }
    [field: SerializeField] public UpgradeView UpgradeUIPrefab { get; private set; }
    [field: SerializeField] public LevelUpView LevelUpUIPrefab { get; private set; }
    [field: SerializeField] public CharacterSelectView CharacterSelectUI { get; private set; }

    public override string ToString()
        => $"Static data";

    public static class Constants
    {
        private static readonly Dictionary<StatType, string> StatNames = new()
        {
            [StatType.Health] = "Health",
            [StatType.Damage] = "Damage",
            [StatType.Defense] = "Defense",
            [StatType.MoveSpeed] = "Movement speed",
            [StatType.JumpHeight] = "Jump height",
            [StatType.projectileSpeed] = "Speed of all projectiles",
            [StatType.projectileScale] = "Radius of all projectiles",
            [StatType.splashRadius] = "Radius of all splash attacks",
        };

        public static float SecondsInMinute = 60f;
        public static int PlayerLayer = LayerMask.NameToLayer("player");
        public static int EnemyLayer = LayerMask.NameToLayer("Enemy");
        public static string EmptyTag = "Untagged";

        public static string NameOfStat(StatType type)
        {
            if (!StatNames.ContainsKey(type))
                return $"Unknown for type of stat \"{type}\"";

            return StatNames[type];
        }
    }
}
