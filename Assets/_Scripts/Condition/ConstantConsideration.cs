using Mirror;
using System;
using UnityEngine;

[Serializable]
public class ConstantConsideration : Consideration
{
    [SerializeField, Range(0f, 1f)] private float _amount = 0.2f;

    public override float Evaluate(NetworkBehaviour sender, NetworkBehaviour target) => _amount;
}
