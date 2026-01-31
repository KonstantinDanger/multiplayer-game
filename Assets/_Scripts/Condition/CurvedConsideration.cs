using System;
using UnityEngine;

[Serializable]
public abstract class CurvedConsideration : Consideration
{
    [field: SerializeField]
    public AnimationCurve ConsiderationCurve { get; private set; }
        = new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(1, 1));
}
