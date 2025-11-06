using System;
using UnityEngine;

[Serializable]
public struct Damage
{
    public float Amount;
    public LayerMask AttackLayers;
    public DamageType Type;

    public GameObject Sender { get; set; }
    public GameObject Receiver { get; set; }
    public Vector3 AttackDirection { get; set; }
}
