using System;
using UnityEngine;

public abstract class Enemy : Entity, IInputBrain
{
    [SerializeField] private EnemyConfig _config;

    [Header("Enemy Components")]
    [SerializeReference, SubclassSelector] protected IDetectable<Player> Detector;
    [SerializeReference, SubclassSelector] protected IAggroHandler AggroHandler;
    [SerializeReference, SubclassSelector] protected IPatrol PatrolStrategy;

    [SerializeField] private AIBrain _aiBrain;

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
    public Entity Target { get; protected set; }

    protected override IInputBrain SetInputBrain() => this;

    protected override void OnAwake()
    {
        {
            //AbilityUser.Initialize(_config
            //        .Abilities
            //        .ToList());
        }

        _aiBrain.Initialize(_config.AIActions);
    }

    protected override void OnStart()
    {
        base.OnStart();

        PatrolStrategy?.Initialize(Movable, transform, transform);
    }

    protected override void Update()
    {
        //base.Update();

        _aiBrain.OnUpdate(Time.deltaTime, AggroHandler?.CurrentTarget);
        PatrolStrategy?.Update(Time.deltaTime);

        DetectionTimer += Time.deltaTime;

        if (DetectionTimer >= Config.DetectionInterval)
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

        Entity nearestPlayer = Detector.DetectNearestPlayer(Config.DetectionRadius);

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

    protected virtual void PerformAbility(Entity target)
    {
        //if (AbilityUser.)

        //if (!AttackStrategy.InProcess && IsInAttackRange(target, AttackStrategy.AttackRange))
        //{
        //    AttackStrategy.Apply(sender: this, target);
        //    AggroHandler?.RefreshAggro();
        //}
    }

    protected bool IsInAttackRange(Entity target, float range)
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        return distanceToTarget <= range;
    }

    public void StopMovement() { }

    protected override void OnDamageTaken(Damage damage)
    {
        base.OnDamageTaken(damage);

        PatrolStrategy?.ResetPatrol();
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
