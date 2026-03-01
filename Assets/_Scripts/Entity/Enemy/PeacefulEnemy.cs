using UnityEngine;

public class PeacefulEnemy : Enemy
{
    [SerializeField] private SiblingNotifier _notifier;

    protected override void OnPlayerDetected(Entity player) { }

    protected override void OnDamageTaken(Damage damage)
    {
        if (!damage.Sender.TryGetComponent(out Entity attacker))
            return;

        if (_notifier != null)
            _notifier.NotifySiblings(attacker);

        FleeFrom(attacker);
    }

    public void OnSiblingAttacked(Entity attacker)
        => FleeFrom(attacker);

    private void FleeFrom(Entity attacker)
    {
        if (!transform.TryGetComponent(out ITargetTrackingMemory trackingMemory))
            return;

        trackingMemory.Memorize(attacker);
    }

    protected override void UpdateBehavior() { }
}
