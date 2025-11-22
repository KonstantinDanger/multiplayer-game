using System;
using UnityEngine;

[Serializable]
public class AggroHandler : IAggroHandler
{
    [SerializeField] private float aggroTimeout = 8f;
    [SerializeField] private float aggroRange = 15f;
    [SerializeField] private Transform _selfTransform;

    private Entity currentTarget;
    private float aggroTimer;
    private bool isAggroed;

    public bool IsAggroed => isAggroed;
    public Entity CurrentTarget => currentTarget;
    public float AggroRange => aggroRange;

    public void Aggro(Entity target)
    {
        if (target == null) return;

        currentTarget = target;
        isAggroed = true;
        aggroTimer = aggroTimeout;
    }

    public void Unaggro()
    {
        currentTarget = null;
        isAggroed = false;
        aggroTimer = 0f;
    }

    public void OnUpdate(float deltaTime)
    {
        if (!isAggroed || currentTarget == null) return;

        float distanceToTarget = Vector3.Distance(_selfTransform.position, currentTarget.transform.position);

        // If target is out of range, decrease timer
        if (distanceToTarget > aggroRange)
        {
            aggroTimer -= deltaTime;

            if (aggroTimer <= 0f)
            {
                Unaggro();
            }
        }
        else
        {
            // Reset timer if target comes back in range
            aggroTimer = aggroTimeout;
        }
    }

    public void RefreshAggro()
    {
        if (isAggroed)
            aggroTimer = aggroTimeout;
    }
}