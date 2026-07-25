using System.Collections.Generic;
using UnityEngine;

public abstract class TargetDetector
{
    [field: SerializeField] public Transform Origin { get; set; }
    [SerializeField] protected LayerMask layersToDetect;
    [SerializeField] protected bool _includeSender;

    public abstract int DetectAllTargets(
        GameObject sender,
        Vector3 origin,
        Vector3 direction,
        float detectionRadius,
        out List<GameObject> targets,
        bool includeSender = false);
}
