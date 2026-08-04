using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetDetectorNone : TargetDetector
{
    public override int DetectAllTargets(GameObject sender, Vector3 origin, Vector3 direction, float detectionRadius, out List<GameObject> targets)
    {
        targets = new();
        return 0;
    }
}
