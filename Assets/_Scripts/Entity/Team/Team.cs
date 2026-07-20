public struct Team
{
    public int Id;

    public const int EnemyTeam = 1;

    public static bool operator ==(Team team1, Team team2)
        => team1.Id == team2.Id;

    public static bool operator !=(Team team1, Team team2)
        => team1.Id != team2.Id;

    public override readonly string ToString() => $"{Id}";
}
