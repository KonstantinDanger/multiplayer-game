using System;

[System.Serializable]
public class GameMatchData
{
    public PlayerMatchSummaryData[] PlayersSummary;
    public string Winner; //Player id
    public string Loser; //Player id
    public float MatchTime;
    public DateTime MatchDate;

    public override string ToString()
    {
        string playersSummary = "";

        foreach (var summary in PlayersSummary)
            playersSummary += $"{summary}\n\n";

        return $"" +
        $"Winner: {Winner}\n" +
        $"Loser: {Loser}\n" +
        $"Match time: {MatchTime}\n" +
        $"Match date: {MatchDate}\n\n" +
        $"Players summary:\n" +
        $"{playersSummary}";

    }
}