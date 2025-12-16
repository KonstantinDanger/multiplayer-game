using AYellowpaper;
using Mirror;
using System;
using UnityEngine;

public class Entity : NetworkBehaviour, IDisposable
{
    public EntityStats Stats = new(Utils.DefaultEntityStats());

    [SerializeField] private InterfaceReference<IMovable> _movement;
    [SerializeField] private InterfaceReference<IRotatable> _rotatable;
    [SerializeField] private InterfaceReference<IDamageable> _damageable;

    [field: SerializeField] public MovementConfig MovementConfig { get; private set; }
    [field: SerializeField] public RotationConfig RotationConfig { get; private set; }
    [field: SerializeField] public DamageSystemConfig DamageSystemConfig { get; private set; }

    public IInputBrain InputBrain { get; private set; }
    public IMovable Movable => _movement.Value;
    public IRotatable Rotatable => _rotatable.Value;
    public IDamageable Damageable => _damageable.Value;

    private Vector2 MovementInput => InputBrain.MovementVector;
    private Vector2 RotationInput => InputBrain.Rotation;

    private void Awake()
    {
        InputBrain = SetInputBrain();
        Damageable.Initialize(DamageSystemConfig.BaseHp, null);

        OnAwake();
    }

    private void Start()
        => OnStart();

    private void OnEnable()
    {
        InputBrain.Enable();

        InputBrain.JumpAction += HandleJump;
        Damageable.OnDamageTaken += OnDamageTaken;
        Damageable.OnDemise += OnDemise;

        HandleOnEnable();
    }

    private void OnDisable()
    {
        InputBrain.Disable();

        InputBrain.JumpAction -= HandleJump;
        Damageable.OnDamageTaken -= OnDamageTaken;
        Damageable.OnDemise -= OnDemise;

        HandleOnDisable();
    }

    protected virtual IInputBrain SetInputBrain()
        => null;

    protected virtual void Update()
    {
        InputBrain.Update();

        Rotatable.Rotate(RotationInput, RotationConfig.RotationSpeed);
    }

    protected virtual void OnDamageTaken(Damage damage) { }

    protected virtual void OnDemise(Damage damage) { }

    protected virtual void HandleOnEnable() { }

    protected virtual void HandleOnDisable() { }

    protected virtual void OnAwake() { }

    protected virtual void OnStart() { }

    protected virtual void HandleJump()
        => Movable.Jump(MovementConfig.JumpHeight, MovementConfig.Gravity);

    public virtual void Dispose() { }
}
