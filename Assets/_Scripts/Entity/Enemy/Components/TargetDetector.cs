using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetDetector : NetworkBehaviour, ITargetDetector
{
    [SerializeField] private LayerMask _layersToDetect;
    [SerializeField] private Transform _transform;

    private readonly Collider[] _targets = new Collider[8];

    public Entity DetectNearestTarget(float detectionRadius)
    {
        var entities = DetectAllEntities(detectionRadius);

        if (entities.Count == 0)
            return null;

        Entity nearest = null;
        float minDist = float.MaxValue;

        foreach (var entity in entities)
        {
            float dist = Vector3.Distance(_transform.position, entity.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = entity;
            }
        }

        return nearest;
    }

    private List<Entity> DetectAllEntities(float detectionRadius)
    {
        int targets = Physics.OverlapSphereNonAlloc(_transform.position, detectionRadius, _targets, _layersToDetect);
        List<Entity> entities = new();

        for (int i = 0; i < targets; i++)
            if (_targets[i].TryGetComponent(out Entity entity))
                if (entity.gameObject != gameObject)
                    entities.Add(entity);

        if (targets != 0)
        {
            entities.ForEach(e => UnityEngine.Debug.Log(e.name));

        }

        return entities;
    }

    public bool IsInFieldOfView(Entity target, float fovAngle, float maxDistance)
    {
        Vector3 dirToTarget = (target.transform.position - _transform.position).normalized;
        float distance = Vector3.Distance(_transform.position, target.transform.position);

        if (distance > maxDistance)
            return false;

        float angle = Vector3.Angle(_transform.forward, dirToTarget);
        return angle <= fovAngle / 2f;
    }

    public void ChangeTargetLayers(LayerMask layersToDetect)
        => _layersToDetect = layersToDetect;
}
