using UnityEngine;

public struct ProjectileData
{
    public float Speed;
    public uint SenderId;
    public Vector3 Direction;

    public readonly Collider Collider; //Remove collider
    public readonly GameObject Sender => Utils.NetIdToGameObject(SenderId);

    public ProjectileData(Collider col)
    {
        Speed = 0;
        SenderId = default;
        Direction = Vector3.zero;
        Collider = col;
    }
}
