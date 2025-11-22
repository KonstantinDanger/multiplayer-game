using UnityEngine;

public class PeacefulEnemy : Enemy
{
    [Header("Peaceful Enemy Settings")]
    [SerializeField] private float _fleeSpeed = 6f;
    [SerializeField] private float _fleeDistance = 15f;
    [SerializeField] private float _fleeDuration = 5f;
    [SerializeField] private SiblingNotifier _notifier;
    [SerializeReference, SubclassSelector] private IFleeBehaviour _fleeBehaviour;

    private bool _isFleeing;
    private float _fleeTimer;
    private Entity _fleeTarget;

    protected override void Update()
    {
        base.Update();

        if (_isFleeing)
        {
            _fleeTimer -= Time.deltaTime;
            if (_fleeTimer <= 0f)
            {
                _isFleeing = false;
                _fleeTarget = null;
            }
        }
    }

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

    private void FleeFrom(Entity target)
    {
        _isFleeing = true;
        _fleeTimer = _fleeDuration;
        _fleeTarget = target;
    }

    protected override void UpdateBehavior()
    {
        if (_isFleeing && _fleeTarget != null)
        {
            Vector3 fleeDestination = _fleeBehaviour.GetFleeDestinationFrom(_fleeTarget.transform, transform, _fleeDistance);
            Vector3 fleeDirection = fleeDestination - transform.position;

            Movable.Move(fleeDestination, _fleeSpeed);

            Rotatable?.Rotate(fleeDirection, RotationConfig.RotationSpeed);
        }
        else
        {
            StopMovement();
        }
    }
}
