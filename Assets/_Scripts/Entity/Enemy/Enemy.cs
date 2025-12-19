using System;
using UnityEngine;

public abstract class Enemy : Entity, IInputBrain
{
    [SerializeField] private EnemyConfig _config;
    [SerializeField] protected float _detectionRadius = 10f;
    [SerializeField] protected float _detectionInterval = 5f;

    [Header("Enemy Components")]
    [SerializeReference, SubclassSelector] protected IDetectable<Player> Detector;
    [SerializeReference, SubclassSelector] protected IAggroHandler AggroHandler;
    [SerializeReference, SubclassSelector] protected IAttackStrategy AttackStrategy;
    [SerializeReference, SubclassSelector] protected IPatrol _patrolStrategy;

    public EnemyConfig Config => _config;
    public IAggroHandler AggroSystem => AggroHandler;

    #region InputBrain fields
    public Vector2 MovementVector => throw new NotImplementedException();
    public Vector2 Rotation => throw new NotImplementedException();
    public bool IsSprinting => throw new NotImplementedException();

    public event Action JumpAction;
    public event Action AttackAction;
    public event Action<int> AbilityAction;
    #endregion

    protected float DetectionTimer;
    protected Entity Target;

    protected override IInputBrain SetInputBrain() => this;

    protected override void OnStart()
    {
        base.OnStart();

        _patrolStrategy?.Initialize(Movable, transform, transform);
    }

    protected override void Update()
    {
        //base.Update();

        _patrolStrategy?.Update(Time.deltaTime);

        DetectionTimer += Time.deltaTime;

        if (DetectionTimer >= _detectionInterval)
        {
            DetectPlayers();
            DetectionTimer = 0f;
        }

        AggroHandler?.OnUpdate(Time.deltaTime);
        UpdateBehavior();
    }

    protected virtual void DetectPlayers()
    {
        if (AggroHandler != null && AggroHandler.IsAggroed)
            return;

        Entity nearestPlayer = Detector.DetectNearestPlayer(_detectionRadius);
        if (nearestPlayer != null)
        {
            OnPlayerDetected(nearestPlayer);
        }
    }

    protected abstract void OnPlayerDetected(Entity player);
    protected abstract void UpdateBehavior();

    protected virtual void ChaseTarget(Entity target)
    {
        if (target == null)
            return;

        Movable.Move(target.transform.position, MovementConfig.Speed);
        Rotatable?.Rotate((target.transform.position - transform.position).normalized, RotationConfig.RotationSpeed);
    }

    protected virtual void AttackTarget(Entity target)
    {
        if (target == null || AttackStrategy == null) return;

        if (AttackStrategy.CanAttack() && AttackStrategy.IsInAttackRange(target))
        {
            AttackStrategy.ExecuteAttack(target);
            AggroHandler?.RefreshAggro();
        }
    }

    public void StopMovement() { }

    protected override void OnDamageTaken(Damage damage)
    {
        base.OnDamageTaken(damage);

        _patrolStrategy?.ResetPatrol();
    }

    protected override void OnDemise(Damage damage)
    {
        base.OnDemise(damage);

        Destroy(gameObject);
    }

    void IInputBrain.Update() => Update();
    public void Enable() { }
    public void Disable() { }
}
