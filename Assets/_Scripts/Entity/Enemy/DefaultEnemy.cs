public class DefaultEnemy : Enemy
{
    protected override void OnTargetDetected(Entity player)
    {
        if (TargetDetector.IsInFieldOfView(player, Config.FieldOfViewAngle, Config.VisionRange))
        {
            if (Config.IsNeutral)
                return;

            if (!TargetTrackingMemory.IsTracking)
            {
                TargetTrackingMemory.Memorize(player);
            }
        }
    }

    protected override void OnDamageTaken(Damage damage)
    {
        base.OnDamageTaken(damage);

        if (!damage.Sender.TryGetComponent(out Player attacker))
            return;

        if (!TargetTrackingMemory.IsTracking)
        {
            TargetTrackingMemory.Memorize(attacker);
        }
    }
}
