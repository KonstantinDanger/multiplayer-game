using System;
using UnityEngine;

[Serializable]
public struct Damage
{
    public float Amount;
    public LayerMask AttackLayers;
    public DamageType Type;
    public float Range;

    [HideInInspector] public uint SenderNetId;
    [HideInInspector] public uint ReceiverNetId;
    [HideInInspector] public Vector3 AttackDirection;

    public void SetAttackLayers(LayerMask layers)
        => AttackLayers = layers;

    public readonly GameObject Sender
        => Utils.NetIdToGameObject(SenderNetId);
    public readonly GameObject Receiver
        => Utils.NetIdToGameObject(ReceiverNetId);
}
