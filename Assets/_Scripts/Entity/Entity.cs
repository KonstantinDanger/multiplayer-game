using AYellowpaper;
using Mirror;
using System;
using UnityEngine;

public class Entity : NetworkBehaviour, IDisposable, IStatUser
{
    [SyncVar] public Team TeamId = new() { Id = Team.EnemyTeam };

    [SerializeField] private InterfaceReference<IMovable> _movement;
    [SerializeField] private InterfaceReference<IRotatable> _rotatable;
    [SerializeField] private InterfaceReference<IDamageable> _damageable;
    [SerializeField] private InterfaceReference<IAbilityUser> _abilityUser;

    [field: SerializeField] public EntityStats Stats { get; private set; }
    [field: SerializeField] public MovementConfig MovementConfig { get; private set; }
    [field: SerializeField] public RotationConfig RotationConfig { get; private set; }
    [field: SerializeField] public DamageSystemConfig DamageSystemConfig { get; private set; }

    public IMovable Movable => _movement.Value;
    public IRotatable Rotatable => _rotatable.Value;
    public IDamageable Damageable => _damageable.Value;
    protected IAbilityUser AbilityUser => _abilityUser.Value;

    public override void OnStartServer()
    {
        base.OnStartServer();

        Damageable.Initialize(DamageSystemConfig.BaseHealth, DamageSystemConfig.DamageHandlers);
        Stats.Initialize(Utils.DefaultEntityStats());

        OnEntityStartServer();
    }

    private void OnEnable()
    {
        Damageable.ServerOnDamageTaken += OnDamageTaken;
        Damageable.ServerOnDemise += OnDemise;
        Stats.OnStatChange += HandleStatChange;

        HandleOnEnable();
    }

    private void OnDisable()
    {
        Damageable.ServerOnDamageTaken -= OnDamageTaken;
        Damageable.ServerOnDemise -= OnDemise;
        Stats.OnStatChange -= HandleStatChange;

        HandleOnDisable();
    }

    protected virtual void Update()
    { }

    [Command(requiresAuthority = false)]
    private void HandleStatChange(StatType type, float statMultiplier)
    {
        IStatConsumer[] statConsumers = GetComponents<IStatConsumer>();

        for (int i = 0; i < statConsumers.Length; i++)
        {
            IStatConsumer component = statConsumers[i];

            if (component == null)
                continue;

            if (component.StatModifier.Stat != type)
                continue;

            StatModifier modifier = component.StatModifier;
            modifier.Multiplier = statMultiplier;
            component.StatModifier = modifier;
        }
    }

    protected virtual void OnEntityStartServer() { }

    protected virtual void OnDamageTaken(Damage damage) { }

    protected virtual void OnDemise(Damage damage) { }

    protected virtual void HandleOnEnable() { }

    protected virtual void HandleOnDisable() { }

    public virtual void Dispose() { }
}
