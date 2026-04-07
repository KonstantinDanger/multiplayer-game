public class DefaultEnemy : Enemy
{
    protected override void OnDamageTaken(Damage damage)
    {
        base.OnDamageTaken(damage);

        if (!damage.Sender.TryGetComponent(out Entity attacker))
            return;

        if (!TargetTrackingMemory.IsTracking)
        {
            TargetTrackingMemory.Memorize(attacker);
        }
    }
}
