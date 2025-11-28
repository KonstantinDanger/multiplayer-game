[System.Serializable]
public class PlayerMatchSummaryData
{
    public string UsedClassName = "Unknown";
    public float DealtDamage = 0;
    public int ReachedLevel = 1;
    public float TotalXpReceived = 0;
    public int DeathsCount = 0;
    public float DamageTaken = 0;
    public float ReceivedCurrency = 0;

    public override string ToString() =>
        $"UsedClassName: {UsedClassName}\n" +
        $"DealtDamage: {DealtDamage}\n" +
        $"ReachedLevel: {ReachedLevel}\n" +
        $"TotalXpReceived: {TotalXpReceived}\n" +
        $"DeathsCount: {DeathsCount}\n" +
        $"DamageTaken: {DamageTaken}\n" +
        $"ReceivedCurrency: {ReceivedCurrency}\n";
}