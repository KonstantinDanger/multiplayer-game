using UnityEngine;

[RequireComponent(typeof(FlightController))]
public class SwarmEnemy : Enemy
{
    [Header("Swarm Settings")]
    [SerializeField] private float _swarmRadius = 5f;
    [SerializeField] private float _separationForce = 2f;
    [SerializeField] private float _cohesionForce = 1f;
    [SerializeField] private float _alignmentForce = 1f;
    [SerializeField] private float _targetForce = 3f;

    [SerializeField] private FlightController _flightController;

    private SwarmManager _swarmManager;

    public void Initialize(SwarmManager swarmManager)
        => _swarmManager = swarmManager;

    protected override void Update()
    {
        base.Update();
        _flightController.MaintainFlightHeight();
    }

    protected override void OnPlayerDetected(Entity player)
    {
        if (!AggroHandler.IsAggroed)
        {
            AggroHandler.Aggro(player);
            TargetPlayer = player;

            _swarmManager.AlertSwarm(player);
        }
    }

    protected override void UpdateBehavior()
    {
        if (!AggroHandler.IsAggroed || TargetPlayer == null)
        {
            FlyIdle();
            return;
        }

        Vector3 swarmDirection = CalculateSwarmBehavior();
        Vector3 targetDirection = (TargetPlayer.transform.position - transform.position).normalized;

        Vector3 finalDirection = (swarmDirection + targetDirection * _targetForce).normalized;
        Vector3 targetPosition = transform.position + finalDirection * 5f;

        _flightController.FlyTowards(targetPosition);

        // Attack if in range
        if (AttackStrategy != null && AttackStrategy.IsInAttackRange(TargetPlayer))
        {
            AttackTarget(TargetPlayer);
        }
    }

    private Vector3 CalculateSwarmBehavior()
    {
        SwarmEnemy[] nearbySwarm = FindNearbySwarmMembers();

        if (nearbySwarm.Length == 0)
            return Vector3.zero;

        Vector3 separation = CalculateSeparation(nearbySwarm);
        Vector3 cohesion = CalculateCohesion(nearbySwarm);
        Vector3 alignment = CalculateAlignment(nearbySwarm);

        return separation * _separationForce + cohesion * _cohesionForce + alignment * _alignmentForce;
    }

    private SwarmEnemy[] FindNearbySwarmMembers()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _swarmRadius);
        System.Collections.Generic.List<SwarmEnemy> nearby = new System.Collections.Generic.List<SwarmEnemy>();

        foreach (var hit in hits)
        {
            SwarmEnemy swarm = hit.GetComponent<SwarmEnemy>();
            if (swarm != null && swarm != this)
                nearby.Add(swarm);
        }

        return nearby.ToArray();
    }

    private Vector3 CalculateSeparation(SwarmEnemy[] nearby)
    {
        Vector3 separationVector = Vector3.zero;

        foreach (var member in nearby)
        {
            Vector3 diff = transform.position - member.transform.position;
            separationVector += diff.normalized / diff.magnitude;
        }

        return separationVector.normalized;
    }

    private Vector3 CalculateCohesion(SwarmEnemy[] nearby)
    {
        Vector3 center = Vector3.zero;

        foreach (var member in nearby)
            center += member.transform.position;

        center /= nearby.Length;

        return (center - transform.position).normalized;
    }

    private Vector3 CalculateAlignment(SwarmEnemy[] nearby)
    {
        Vector3 avgDirection = Vector3.zero;

        foreach (var member in nearby)
            avgDirection += member.transform.forward;

        return (avgDirection / nearby.Length).normalized;
    }

    private void FlyIdle() => _flightController.MaintainFlightHeight();

    protected override void OnDemise(Damage damage)
    {
        base.OnDemise(damage);

        _swarmManager.UnregisterSwarmMember(this);
    }

    private void OnDestroy()
        => _swarmManager.UnregisterSwarmMember(this);
}


