using AYellowpaper;
using Mirror;
using System;
using UnityEngine;

[RequireComponent(typeof(IMovable))]
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
        InputBrain = new PlayerInput();
        Damageable.Initialize(DamageSystemConfig.BaseHp, null);

        OnAwake();
    }

    private void Start()
        => OnStart();

    private void OnEnable()
    {
        InputBrain.OnEnable();

        InputBrain.JumpAction += HandleJump;
        Damageable.OnDamageTaken += OnDamageTaken;
        Damageable.OnDemise += OnDemise;

        HandleOnEnable();
    }

    private void OnDisable()
    {
        InputBrain.OnDisable();

        InputBrain.JumpAction -= HandleJump;
        Damageable.OnDamageTaken -= OnDamageTaken;
        Damageable.OnDemise -= OnDemise;

        HandleOnDisable();
    }

    protected virtual void Update()
    {
        InputBrain.Update();

        float currentSpeed =
            (InputBrain.IsSprinting && MovementInput != Vector2.zero) ?
            MovementConfig.SprintSpeed : MovementConfig.Speed;

        Vector3 movementDirection = GetMovementDirection(MovementInput);
        Movable.Move(movementDirection, currentSpeed, MovementConfig.MovementSmoothness);
        Movable.ApplyGravity(MovementConfig.Gravity, MovementConfig.MaxFallSpeed);

        Rotatable.Rotate(RotationInput, RotationConfig.RotationSpeed);
    }

    private Vector3 GetMovementDirection(Vector2 movementVector)
    {
        Vector3 v = transform.right * movementVector.x + transform.forward * movementVector.y;
        v = Vector3.ClampMagnitude(v, 1f);
        return v;
    }

    protected virtual void OnDamageTaken(Damage damage) { }

    protected virtual void OnDemise(Damage damage) { }

    protected virtual void HandleOnEnable()
    {

    }

    protected virtual void HandleOnDisable()
    {

    }

    protected virtual void OnAwake()
    {

    }

    protected virtual void OnStart()
    {

    }

    protected virtual void HandleJump()
        => Movable.Jump(MovementConfig.JumpHeight, MovementConfig.Gravity);

    public virtual void Dispose() { }
}
