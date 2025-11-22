using System;
using UnityEngine;

[Serializable]
public struct Damage
{
    public float Amount;
    public LayerMask AttackLayers;
    public DamageType Type;
    public float Range;

    //net ids instead of refs
    [HideInInspector] public uint SenderNetId;
    [HideInInspector] public uint ReceiverNetId;
    [HideInInspector] public Vector3 AttackDirection;

    public readonly GameObject Sender
        => Utils.NetIdToGameObject(SenderNetId);
    public readonly GameObject Receiver
        => Utils.NetIdToGameObject(ReceiverNetId);
}
