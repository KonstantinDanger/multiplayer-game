public struct Team
{
    public int Id;

    public const int EnemyTeam = 1;

    public override readonly string ToString() => $"{Id}";
}
