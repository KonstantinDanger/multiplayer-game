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
    public TargetDetectionConfig DetectionConfig => _enemyConfig.TargetDetectionConfig;

    protected ITargetTrackingMemory TargetTrackingMemory => TargetTrackingMemoryRef.Value;
    public ITargetDetector TargetDetector => TargetDetectorRef.Value;

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

    private void Awake()
    {
        _aiBrain.Initialize(this, _enemyConfig.AIActions);
        DetectionTimer = DetectionConfig.DetectionInterval;

    }

    protected override void Update()
    {
        _aiBrain.OnUpdate(Time.deltaTime, TargetTrackingMemory.Target);

        DetectionTimer += Time.deltaTime;

        if (DetectionTimer >= DetectionConfig.DetectionInterval)
        {
            DetectTargets();
            DetectionTimer = 0f;
        }
    }

    protected virtual void DetectTargets()
    {
        if (TargetTrackingMemory.Target)
            return;

        Entity nearest = TargetDetector.DetectNearestTarget(DetectionConfig.DetectionRadius, DetectionConfig.FieldOfViewAngle);

        if (nearest != null)
        {
            if (Vector3.Distance(transform.position, nearest.transform.position) > DetectionConfig.VisionRange)
                return;

            OnTargetDetected(nearest);
        }
    }

    protected virtual void OnTargetDetected(Entity entity)
        => TargetTrackingMemory.Memorize(entity);

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
