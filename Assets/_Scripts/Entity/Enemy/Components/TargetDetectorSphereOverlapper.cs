using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TargetDetectorSphereOverlapper : TargetDetector
{
    [SerializeField, Range(0f, 360f)] private float _fieldOfViewAngle;
    [SerializeField] private bool _ignoreFieldOfView;

    private readonly Collider[] _targets = new Collider[16];

    public override GameObject DetectNearestTarget(GameObject sender, Vector3 origin, Vector3 direction, float detectionRadius, bool includeSender = false)
        => DetectAllTargets(sender, origin, direction, detectionRadius, includeSender).FirstOrDefault();

    public override IEnumerable<GameObject> DetectAllTargets(GameObject sender, Vector3 origin, Vector3 direction, float detectionRadius, bool includeSender = false)
    {
        int targets = Physics.OverlapSphereNonAlloc(origin, detectionRadius, _targets, layersToDetect);
        List<GameObject> entities = new();

        for (int i = 0; i < targets; i++)
        {
            GameObject currentObject = _targets[i].gameObject;

            if (!includeSender && currentObject == sender)
                continue;

            if (!_ignoreFieldOfView && !IsInFieldOfView(currentObject, _fieldOfViewAngle))
                continue;

            entities.Add(currentObject);
        }

        return entities;
    }

    public bool IsInFieldOfView(GameObject target, float fovAngle)
    {
        Vector3 dirToTarget = (target.transform.position - Origin.position).normalized;
        float angle = Vector3.Angle(Origin.forward, dirToTarget);
        return angle <= fovAngle / 2f;
    }
}
