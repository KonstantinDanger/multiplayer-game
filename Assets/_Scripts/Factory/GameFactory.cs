using UnityEngine;

public class GameFactory
{
    public Player SpawnPlayer(Player player, Transform point)
        => Object.Instantiate(player, point.position, Quaternion.identity);
}
