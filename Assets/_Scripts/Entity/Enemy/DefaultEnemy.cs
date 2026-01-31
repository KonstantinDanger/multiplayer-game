public class DefaultEnemy : Enemy
{
    //private enum State
    //{
    //    Patrolling,
    //    Chasing,
    //    AbilityUse
    //}

    //private State _currentState = State.Patrolling;

    protected override void OnPlayerDetected(Entity player)
    {
        if (Detector.IsInFieldOfView(player, Config.FieldOfViewAngle, Config.VisionRange))
        {
            if (Config.IsNeutral)
                return;

            if (!AggroHandler.IsAggroed)
            {
                AggroHandler.Aggro(player);
                Target = player;
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
            Target = attacker;
        }
    }

    protected override void UpdateBehavior()
    {
        //if (!AggroHandler.IsAggroed)
        //{
        //    _currentState = State.Patrolling;
        //    Patrol();
        //    return;
        //}

        //Target = AggroHandler.CurrentTarget;

        //if (Target == null)
        //{
        //    _currentState = State.Patrolling;
        //    return;
        //}

        //switch (_currentState)
        //{
        //    case State.Patrolling:
        //        if (AggroHandler.IsAggroed)
        //            _currentState = State.Chasing;
        //        break;

        //    case State.Chasing:
        //        ChaseTarget(Target);

        //        //if (AbilityUser != null)
        //        //    _currentState = State.AbilityUse;
        //        break;

        //    case State.AbilityUse:
        //        StopMovement();
        //        PerformAbility(Target);

        //        //if (AbilityUser != null)
        //        //    _currentState = State.Chasing;
        //        break;
        //}
    }
}
