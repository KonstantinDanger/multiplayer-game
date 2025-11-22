using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerDetector : IDetectable<Player>
{
    [SerializeField] private LayerMask _layersToDetect;
    [SerializeField] private Transform _transform;

    public Player DetectNearestPlayer(float detectionRadius)
    {
        var players = DetectAllPlayers(detectionRadius);

        if (players.Count == 0)
            return null;

        Player nearest = null;
        float minDist = float.MaxValue;

        foreach (var player in players)
        {
            float dist = Vector3.Distance(_transform.position, player.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = player;
            }
        }

        return nearest;
    }

    public List<Player> DetectAllPlayers(float detectionRadius)
    {
        Collider[] hits = Physics.OverlapSphere(_transform.position, detectionRadius, _layersToDetect);
        List<Player> players = new();

        foreach (var hit in hits)
            if (hit.TryGetComponent(out Player player))
                players.Add(player);

        return players;
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
