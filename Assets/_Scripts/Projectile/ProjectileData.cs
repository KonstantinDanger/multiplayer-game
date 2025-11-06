using UnityEngine;

public struct ProjectileData
{
    public float Speed;
    public GameObject Sender;
    public Vector3 Direction;

    public readonly Collider Collider;

    public ProjectileData(Collider col)
    {
        Speed = 0;
        Sender = null;
        Direction = Vector3.zero;
        Collider = col;
    }
}
