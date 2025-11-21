using Mirror;
using UnityEngine;

public class SwarmAgent : NetworkBehaviour
{
    [Header("Swarm Behavior")]
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;
    public float targetWeight = 2.0f;

    public float separationRadius = 2f;
    public float neighborRadius = 5f;

    private Enemy enemy;
    private SwarmManager swarmManager;

    public SwarmManager SwarmManager => swarmManager;

    private void Awake()
        => enemy = GetComponent<Enemy>();

    public void Initialize(SwarmManager manager)
        => swarmManager = manager;

    private void Update()
    {
        if (enemy == null || !enemy.IsChasing || enemy.Target == null)
            return;

        IMovable movable = enemy.Movable;
        IRotatable rotatable = enemy.Rotatable;

        Vector3 desiredDirection = CalculateSwarmDirection();

        if (movable != null && desiredDirection != Vector3.zero)
        {
            movable.Move(desiredDirection.normalized, enemy.MovementConfig.Speed, enemy.MovementConfig.MovementSmoothness);
        }

        if (rotatable != null && desiredDirection != Vector3.zero)
        {
            rotatable.Rotate(desiredDirection.normalized, enemy.RotationConfig.RotationSpeed);
        }
    }

    private Vector3 CalculateSwarmDirection()
    {
        Vector3 separation = CalculateSeparation();
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 targetDirection = CalculateTargetDirection();

        // Combine all forces with weights
        Vector3 finalDirection =
            separation * separationWeight +
            alignment * alignmentWeight +
            cohesion * cohesionWeight +
            targetDirection * targetWeight;

        return finalDirection;
    }

    private Vector3 CalculateSeparation()
    {
        Vector3 separationForce = Vector3.zero;
        int neighborCount = 0;

        foreach (SwarmAgent other in swarmManager.GetAgents())
        {
            if (other == this) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);

            if (distance < separationRadius && distance > 0)
            {
                Vector3 away = transform.position - other.transform.position;
                separationForce += away.normalized / distance; // Closer = stronger push
                neighborCount++;
            }
        }

        return neighborCount > 0 ? separationForce / neighborCount : Vector3.zero;
    }

    private Vector3 CalculateAlignment()
    {
        Vector3 averageDirection = Vector3.zero;
        int neighborCount = 0;

        foreach (SwarmAgent other in swarmManager.GetAgents())
        {
            if (other == this) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);

            if (distance < neighborRadius)
            {
                averageDirection += other.transform.forward;
                neighborCount++;
            }
        }

        return neighborCount > 0 ? averageDirection / neighborCount : Vector3.zero;
    }

    private Vector3 CalculateCohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        int neighborCount = 0;

        foreach (SwarmAgent other in swarmManager.GetAgents())
        {
            if (other == this) continue;

            float distance = Vector3.Distance(transform.position, other.transform.position);

            if (distance < neighborRadius)
            {
                centerOfMass += other.transform.position;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            centerOfMass /= neighborCount;
            return (centerOfMass - transform.position).normalized;
        }

        return Vector3.zero;
    }

    private Vector3 CalculateTargetDirection()
    {
        if (enemy.Target == null) return Vector3.zero;

        return (enemy.Target.position - transform.position).normalized;
    }
}
