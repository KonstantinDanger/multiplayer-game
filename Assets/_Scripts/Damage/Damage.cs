using System;
using UnityEngine;

[Serializable]
public struct Damage
{
    public float Amount;
    public Team Team;
    public DamageType Type;
    public float Range;

    [HideInInspector] public uint SenderNetId;
    [HideInInspector] public uint ReceiverNetId;
    [HideInInspector] public Vector3 AttackDirection;

    public readonly GameObject Sender
        => Utils.NetIdToGameObject(SenderNetId);
    public readonly GameObject Receiver
        => Utils.NetIdToGameObject(ReceiverNetId);

    public override readonly string ToString()
        => $"Damage amount: {Amount} | Type: {Type} | Range: {Range} | Team: {Team}";
}
