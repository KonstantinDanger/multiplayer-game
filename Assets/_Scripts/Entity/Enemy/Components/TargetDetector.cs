using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetDetector : NetworkBehaviour, ITargetDetector
{
    [SerializeField] private LayerMask _layersToDetect;
    [SerializeField] private Transform _transform;

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
        Collider[] hits = Physics.OverlapSphere(_transform.position, detectionRadius, _layersToDetect);
        List<Entity> entities = new();

        foreach (var hit in hits)
            if (hit.TryGetComponent(out Player player))
                entities.Add(player);

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
}
