using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetDetector : ITargetDetector
{
    [field: SerializeField] public Transform Origin { get; set; }
    [SerializeField] private LayerMask _layersToDetect;

    private readonly Collider[] _targets = new Collider[8];

    public GameObject DetectNearestTarget(float detectionRadius, float fieldOfViewAngle)
    {
        var entities = DetectAllEntities(Origin, detectionRadius, fieldOfViewAngle);

        if (entities.Count == 0)
            return null;

        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (var entity in entities)
        {
            float dist = Vector3.Distance(Origin.position, entity.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = entity;
            }
        }

        return nearest;
    }

    private List<GameObject> DetectAllEntities(Transform origin, float detectionRadius, float fieldOfView)
    {
        int targets = Physics.OverlapSphereNonAlloc(origin.position, detectionRadius, _targets, _layersToDetect);
        List<GameObject> entities = new();

        for (int i = 0; i < targets; i++)
            if (_targets[i].gameObject != origin.gameObject)
                if (IsInFieldOfView(_targets[i].gameObject, fieldOfView))
                    entities.Add(_targets[i].gameObject);

        return entities;
    }

    public bool IsInFieldOfView(GameObject target, float fovAngle)
    {
        Vector3 dirToTarget = (target.transform.position - Origin.position).normalized;
        float angle = Vector3.Angle(Origin.forward, dirToTarget);
        return angle <= fovAngle / 2f;
    }

    public void ChangeTargetLayers(LayerMask layersToDetect)
        => _layersToDetect = layersToDetect;
}
