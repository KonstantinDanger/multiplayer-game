using UnityEngine;

[RequireComponent(typeof(FlightController))]
public class FlyingEnemy : DefaultEnemy
{
    [Header("Flying Settings")]
    [SerializeField] private float _meleeRange = 2f;
    [SerializeField] private float _retreatDistance = 8f;
    [SerializeField] private float _projectileCooldown = 3f;

    [SerializeField] private FlightController _flightController;
    [SerializeReference, SubclassSelector] private IAttackStrategy _projectileAttack;

    private float _projectileTimer;

    private enum State
    {
        Patrolling,
        Approaching,
        MeleeAttacking,
        Retreating,
        RangedAttacking
    }

    private State _currentState = State.Patrolling;

    protected override void Update()
    {
        base.Update();

        _flightController.MaintainFlightHeight();

        _projectileTimer -= Time.deltaTime;
    }

    protected override void UpdateBehavior()
    {
        if (!AggroHandler.IsAggroed)
        {
            _currentState = State.Patrolling;
            FlyPatrol();
            return;
        }

        Target = AggroHandler.CurrentTarget;

        if (Target == null)
        {
            _currentState = State.Patrolling;
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);

        switch (_currentState)
        {
            case State.Patrolling:
                if (AggroHandler.IsAggroed)
                    _currentState = State.Approaching;
                break;

            case State.Approaching:
                _flightController.FlyTowards(Target.transform.position);

                if (distanceToTarget <= _meleeRange)
                    _currentState = State.MeleeAttacking;
                else if (_projectileTimer <= 0f && _projectileAttack != null)
                    _currentState = State.RangedAttacking;
                break;

            case State.MeleeAttacking:
                PerformMeleeAttack();
                _currentState = State.Retreating;
                break;

            case State.Retreating:
                if (!_flightController.IsRetreating)
                    _currentState = State.Approaching;
                break;

            case State.RangedAttacking:
                PerformRangedAttack();
                _currentState = State.Approaching;
                break;
        }
    }

    private void PerformMeleeAttack()
    {
        if (Target != null)
        {
            AttackTarget(Target);
            _flightController.Retreat(Target.transform.position, _retreatDistance);
        }
    }

    private void PerformRangedAttack()
    {
        if (Target != null)
        {
            _projectileAttack.ExecuteAttack(Target);
            _projectileTimer = _projectileCooldown;
        }
    }

    // Simple hovering - can be extended with waypoints
    private void FlyPatrol() =>
        _flightController.MaintainFlightHeight();
}
