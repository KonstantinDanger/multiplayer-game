using System.Collections.Generic;
using UnityEngine;

public abstract class TargetDetector
{
    [field: SerializeField] public Transform Origin { get; set; }
    [SerializeField] protected LayerMask layersToDetect;
    [SerializeField] protected bool _includeSender;

    public abstract GameObject DetectNearestTarget(GameObject sender, Vector3 origin, Vector3 direction, float detectionRadius, bool includeSender = false);
    public abstract IEnumerable<GameObject> DetectAllTargets(GameObject sender, Vector3 origin, Vector3 direction, float detectionRadius, bool includeSender = false);
}
