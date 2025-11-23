using UnityEngine;

public class DefaultEnemy : Enemy
{
    [Header("Aggressive Settings")]
    [SerializeField] private float _fieldOfViewAngle = 120f;
    [SerializeField] private float _visionRange = 15f;
    [SerializeField] private bool _isNeutral;

    private enum State
    {
        Patrolling,
        Chasing,
        Attacking
    }

    private State _currentState = State.Patrolling;

    protected override void OnPlayerDetected(Entity player)
    {
        if (Detector.IsInFieldOfView(player, _fieldOfViewAngle, _visionRange))
        {
            if (_isNeutral)
                return;

            if (!AggroHandler.IsAggroed)
            {
                AggroHandler.Aggro(player);
                TargetPlayer = player;
            }
        }
    }

    protected override void OnDamageTaken(Damage damage)
    {
        base.OnDamageTaken(damage);

        if (!damage.Sender.TryGetComponent(out Player attacker))
            return;

        if (!AggroHandler.IsAggroed)
        {
            AggroHandler.Aggro(attacker);
            TargetPlayer = attacker;
        }
    }

    protected override void UpdateBehavior()
    {
        if (!AggroHandler.IsAggroed)
        {
            _currentState = State.Patrolling;
            Patrol();
            return;
        }

        TargetPlayer = AggroHandler.CurrentTarget;

        if (TargetPlayer == null)
        {
            _currentState = State.Patrolling;
            return;
        }

        switch (_currentState)
        {
            case State.Patrolling:
                if (AggroHandler.IsAggroed)
                    _currentState = State.Chasing;
                break;

            case State.Chasing:
                ChaseTarget(TargetPlayer);

                if (AttackStrategy != null && AttackStrategy.IsInAttackRange(TargetPlayer))
                    _currentState = State.Attacking;
                break;

            case State.Attacking:
                StopMovement();
                AttackTarget(TargetPlayer);

                if (AttackStrategy != null && !AttackStrategy.IsInAttackRange(TargetPlayer))
                    _currentState = State.Chasing;
                break;
        }
    }

    private void Patrol() =>
        _patrolStrategy.SetPatrolOrigin(transform.position);
}
