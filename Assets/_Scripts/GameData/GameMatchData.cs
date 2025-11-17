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
        => $"" +
        $"Winner: {Winner}\n" +
        $"Loser: {Loser}\n" +
        $"Match time: {MatchTime}\n" +
        $"Match date: {MatchDate}";
}