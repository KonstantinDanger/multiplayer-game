using System.Collections.Generic;
using UnityEngine;

public abstract class TargetDetector
{
    [SerializeField] protected LayerMask layersToDetect;
    [SerializeField] protected bool includeSender;

    public abstract int DetectAllTargets(
        GameObject sender,
        Vector3 origin,
        Vector3 direction,
        float detectionRadius,
        out List<GameObject> targets);
}
