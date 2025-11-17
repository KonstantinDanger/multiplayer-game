using System.Collections.Generic;

public class GameData
{
    public Dictionary<PlayerData, PlayerMatchSummaryData> Players;
    public GameMatchData GameMatchData;

    public GameData()
    {
        Players = new();
        GameMatchData = new();
    }
}