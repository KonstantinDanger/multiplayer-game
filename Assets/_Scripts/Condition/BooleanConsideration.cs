using Mirror;
using System;
using UnityEngine;

[Serializable]
public abstract class BooleanConsideration : Consideration
{
    [Header("The fields below represent utility score of consideration if it is true or false")]
    [SerializeField, Range(0f, 1f)] private float _valueOfTrue;
    [SerializeField, Range(0f, 1f)] private float _valueOfFalse;

    public sealed override float Evaluate(NetworkBehaviour sender, NetworkBehaviour target)
        => OnEvaluate(sender, target) ? _valueOfTrue : _valueOfFalse;

    protected abstract bool OnEvaluate(NetworkBehaviour sender, NetworkBehaviour target);
}
