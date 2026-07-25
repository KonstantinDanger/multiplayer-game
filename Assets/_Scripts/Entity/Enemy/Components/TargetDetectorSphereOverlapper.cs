using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetDetectorSphereOverlapper : TargetDetector
{
    [SerializeField, Range(0f, 360f)] private float _fieldOfViewAngle;
    [SerializeField] private bool _ignoreFieldOfView;

    private readonly Collider[] _targets = new Collider[16];

    public override int DetectAllTargets(GameObject sender, Vector3 origin, Vector3 direction, float detectionRadius, out List<GameObject> targets)
    {
        int targetCount = Physics.OverlapSphereNonAlloc(origin, detectionRadius, _targets, layersToDetect);
        targets = new();

        for (int i = 0; i < targetCount; i++)
        {
            GameObject currentObject = _targets[i].gameObject;

            if (!includeSender && currentObject == sender)
                continue;

            if (!_ignoreFieldOfView && !IsInFieldOfView(currentObject, origin, direction, _fieldOfViewAngle))
                continue;

            targets.Add(currentObject);
        }

        return targets.Count;
    }

    public bool IsInFieldOfView(GameObject target, Vector3 origin, Vector3 direction, float fovAngle)
    {
        Vector3 dirToTarget = (target.transform.position - origin).normalized;
        float angle = Vector3.Angle(direction, dirToTarget);
        return angle <= fovAngle / 2f;
    }
}
