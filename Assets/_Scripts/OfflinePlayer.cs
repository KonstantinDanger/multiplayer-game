public class OfflinePlayer
{
    public Player Player { get; private set; }

    public OfflinePlayer(Player player)
        => Player = player;
}
