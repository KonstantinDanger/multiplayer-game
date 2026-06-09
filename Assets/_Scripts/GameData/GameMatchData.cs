using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GameMatchData
{
    private Dictionary<PlayerData, PlayerMatchSummaryData> PlayerSummaries { get; set; } = new();

    public Data MatchData = new();

    public PlayerMatchSummaryData AddNewSummaryForPlayerData(PlayerData playerData)
    {
        PlayerMatchSummaryData summaryData = new();
        PlayerSummaries.Add(playerData, summaryData);
        return summaryData;
    }

    public PlayerMatchSummaryData GetSummaryFor(Player player)
    {
        return PlayerSummaries
            .Where(pair => pair.Key.Name == player.name)
            .Select(pair => pair.Value)
            .FirstOrDefault();
    }

    public override string ToString()
    {
        string playersSummary = "";

        foreach (KeyValuePair<PlayerData, PlayerMatchSummaryData> summary in PlayerSummaries)
        {
            playersSummary += $"Summary for player \"{summary.Key.Name}\":\n";
            playersSummary += $"{summary.Value}\n\n";
        }

        return $"Game match summary:\n" +
        $"winnerSteamName: {MatchData.winnerSteamName}\n" +
        $"loserSteamName: {MatchData.loserSteamName}\n" +
        $"Match time: {MatchData.matchTime}\n" +
        $"Match date: {MatchData.matchDate}\n\n" +
        $"{playersSummary}";
    }

    [Serializable]
    public struct Data
    {
        public string winnerSteamName;
        public string loserSteamName;
        public float matchTime;
        public string matchDate;

        public void SetDate(DateTime date)
            => matchDate = date.ToUniversalTime().ToString("o");
    }
}