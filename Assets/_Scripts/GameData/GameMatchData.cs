[System.Serializable]
public class GameMatchData {
    public PlayerMatchSummary[] PlayersSummary;
    public string Winner; //Player id
    public string Loser; //Player id
    public float MatchTime;
    public DateTime MatchDate;
}