using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CompositionOperation
{
    Sum, Subtraction, Multiplication, Division
}

[Serializable]
public class CompoundConsideration : Consideration
{
    [SerializeReference, SubclassSelector] private List<Consideration> _considerations = new();
    [SerializeField] private CompositionOperation _operation;

    public override float Evaluate(NetworkBehaviour sender, NetworkBehaviour target)
    {
        switch (_operation)
        {
            case CompositionOperation.Sum:
                return _considerations.Sum(cons => cons.Evaluate(sender, target));

            case CompositionOperation.Multiplication:
                return _considerations
                    .Select(consideration => consideration
                    .Evaluate(sender, target))
                    .Aggregate((accumulator, next) => accumulator * next);

            case CompositionOperation.Division:
                return _considerations
                    .Select(consideration => consideration
                    .Evaluate(sender, target))
                    .Aggregate((accumulator, next) => accumulator / next);
        }

        return 0;
    }
}
