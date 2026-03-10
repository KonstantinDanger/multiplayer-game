using AYellowpaper;
using Mirror;
using System;
using UnityEngine;

public class Entity : NetworkBehaviour, IDisposable, IStatUser
{
    public EntityStats Stats { get; } = new(Utils.DefaultEntityStats());

    [SerializeField] private InterfaceReference<IMovable> _movement;
    [SerializeField] private InterfaceReference<IRotatable> _rotatable;
    [SerializeField] private InterfaceReference<IDamageable> _damageable;
    [SerializeField] private InterfaceReference<IAttacker> _attacker;
    [SerializeField] private InterfaceReference<IAbilityUser> _abilityUser;

    [field: SerializeField] public MovementConfig MovementConfig { get; private set; }
    [field: SerializeField] public RotationConfig RotationConfig { get; private set; }
    [field: SerializeField] public DamageSystemConfig DamageSystemConfig { get; private set; }

    public IMovable Movable => _movement.Value;
    public IRotatable Rotatable => _rotatable.Value;
    public IDamageable Damageable => _damageable.Value;
    public IAttacker Attacker => _attacker.Value;
    protected IAbilityUser AbilityUser => _abilityUser.Value;

    private void Awake()
    {
        Damageable.Initialize(DamageSystemConfig.BaseHealth, null);

        OnAwake();
    }

    private void Start()
        => OnStart();

    private void OnEnable()
    {
        Damageable.OnDamageTaken += OnDamageTaken;
        Damageable.OnDemise += OnDemise;
        Stats.OnStatChange += HandleStatChange;

        HandleOnEnable();
    }

    private void OnDisable()
    {
        Damageable.OnDamageTaken -= OnDamageTaken;
        Damageable.OnDemise -= OnDemise;
        Stats.OnStatChange -= HandleStatChange;

        HandleOnDisable();
    }

    protected virtual void Update()
    { }

    private void HandleStatChange(StatType type, float statMultiplier)
    {
        IStatConsumer[] components = GetComponents<IStatConsumer>();

        for (int i = 0; i < components.Length; i++)
        {
            IStatConsumer component = components[i];

            if (component == null)
                continue;

            if (component.StatModifier.Stat != type)
                continue;

            StatModifier modifier = component.StatModifier;
            modifier.Multiplier = statMultiplier;
            component.StatModifier = modifier;
        }
    }

    protected virtual void OnDamageTaken(Damage damage) { }

    protected virtual void OnDemise(Damage damage) { }

    protected virtual void HandleOnEnable() { }

    protected virtual void HandleOnDisable() { }

    protected virtual void OnAwake() { }

    protected virtual void OnStart() { }

    public virtual void Dispose() { }
}
