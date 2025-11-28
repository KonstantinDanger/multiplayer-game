using System.Collections.Generic;
using System.Linq;

public class GameData
{
    private Dictionary<PlayerData, PlayerMatchSummaryData> Players { get; set; } = new();
    public GameMatchData GameMatchData { get; set; } = new();

    public PlayerMatchSummaryData AddPlayerData(PlayerData playerData)
    {
        var summaryData = new PlayerMatchSummaryData();
        Players.Add(playerData, summaryData);
        return summaryData;
    }

    public PlayerMatchSummaryData GetPlayerSummary(Player player)
    {
        return Players
            .Where(pair => pair.Key.Name == player.name)
            .Select(pair => pair.Value)
            .FirstOrDefault();
    }
}