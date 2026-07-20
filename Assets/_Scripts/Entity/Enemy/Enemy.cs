using AYellowpaper;
using UnityEngine;

public abstract class Enemy : Entity
{
    [SerializeField] private EnemyConfig _enemyConfig;

    [Header("Enemy Components")]
    [field: SerializeReference, SubclassSelector]
    public ITargetDetector TargetDetector { get; private set; }

    [SerializeField] private InterfaceReference<ITargetTrackingMemory> TargetTrackingMemoryRef;
    [SerializeField] private AIBrain _aiBrain;

    public EnemyConfig Config => _enemyConfig;
    public TargetDetectionConfig DetectionConfig => _enemyConfig.TargetDetectionConfig;

    protected ITargetTrackingMemory TargetTrackingMemory => TargetTrackingMemoryRef.Value;

    protected float DetectionTimer;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (TargetDetector == null)
            TargetDetector = new TargetDetector();

        if (TargetDetector.Origin == null)
            TargetDetector.Origin = transform;

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

        GameObject nearest = TargetDetector.DetectNearestTarget(DetectionConfig.DetectionRadius, DetectionConfig.FieldOfViewAngle);

        if (nearest == null)
            return;

        if (nearest.TryGetComponent(out Entity entity))
            return;

        if (entity.TeamId.Id == TeamId.Id)
            return;

        if (Vector3.Distance(transform.position, nearest.transform.position) > DetectionConfig.VisionRange)
            return;

        OnTargetDetected(entity);
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
