using AYellowpaper;
using UnityEngine;

public abstract class Enemy : Entity
{
    [SerializeField] private EnemyConfig _enemyConfig;

    [Header("Enemy Components")]
    [SerializeField] private InterfaceReference<ITargetDetector> TargetDetectorRef;
    [SerializeField] private InterfaceReference<ITargetTrackingMemory> TargetTrackingMemoryRef;
    [SerializeField] private AIBrain _aiBrain;

    public EnemyConfig Config => _enemyConfig;

    protected ITargetTrackingMemory TargetTrackingMemory => TargetTrackingMemoryRef.Value;
    protected ITargetDetector TargetDetector => TargetDetectorRef.Value;

    protected float DetectionTimer;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (TargetDetector == null)
            TargetDetectorRef.Value = GetComponent<ITargetDetector>();

        if (TargetTrackingMemory == null)
            TargetTrackingMemoryRef.Value = GetComponent<ITargetTrackingMemory>();

        if (_aiBrain == null)
            _aiBrain = GetComponent<AIBrain>();
    }

    protected override void OnAwake()
        => _aiBrain.Initialize(this, _enemyConfig.AIActions);

    protected override void Update()
    {
        _aiBrain.OnUpdate(Time.deltaTime, TargetTrackingMemory.Target);

        DetectionTimer += Time.deltaTime;

        if (DetectionTimer >= Config.DetectionInterval)
        {
            DetectTargets();
            DetectionTimer = 0f;
        }
    }

    protected virtual void DetectTargets()
    {
        if (TargetTrackingMemory.Target)
            return;

        Entity nearestPlayer = TargetDetector.DetectNearestTarget(Config.DetectionRadius);

        if (nearestPlayer != null)
        {
            OnTargetDetected(nearestPlayer);
        }
    }

    protected abstract void OnTargetDetected(Entity player);

    protected virtual void ChaseTarget(Entity target)
    {
        if (target == null)
            return;

        Movable.Move(target.transform.position, MovementConfig.Speed);
        Rotatable?.Rotate((target.transform.position - transform.position).normalized, RotationConfig.RotationSpeed);
    }

    protected override void OnDemise(Damage damage)
    {
        base.OnDemise(damage);

        Destroy(gameObject);
    }
}
