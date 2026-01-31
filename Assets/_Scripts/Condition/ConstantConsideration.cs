using Mirror;
using System;
using UnityEngine;

[Serializable]
public class ConstantConsideration : Consideration
{
    [SerializeField, Range(0f, 1f)] private float _score = 0.2f;

    public override float Evaluate(NetworkBehaviour sender, NetworkBehaviour target) => _score;
}
