using AYellowpaper;
using Mirror;
using UnityEngine;

[RequireComponent(typeof(IMovable))]
public class Entity : NetworkBehaviour
{
    public EntityStats Stats = new(Utils.DefaultEntityStats());

    [SerializeField] private InterfaceReference<IMovable> _movement;
    [SerializeField] private InterfaceReference<IRotatable> _rotatable;

    [field: SerializeField] public MovementConfig MovementConfig { get; private set; }
    [field: SerializeField] public RotationConfig RotationConfig { get; private set; }

    public IInputBrain InputBrain { get; private set; }
    protected IMovable Movable => _movement.Value;
    protected IRotatable Rotatable => _rotatable.Value;

    private Vector2 MovementInput => InputBrain.MovementVector;
    private Vector2 RotationInput => InputBrain.Rotation;

    private void Awake()
    {
        InputBrain = new PlayerInput();

        OnAwake();
    }

    private void Start()
        => OnStart();

    private void OnEnable()
    {
        InputBrain.OnEnable();

        InputBrain.JumpAction += HandleJump;

        HandleOnEnable();
    }

    private void OnDisable()
    {
        InputBrain.OnDisable();

        InputBrain.JumpAction -= HandleJump;

        HandleOnDisable();
    }

    protected virtual void Update()
    {
        InputBrain.Update();

        float currentSpeed =
            (InputBrain.IsSprinting && MovementInput != Vector2.zero) ?
            MovementConfig.SprintSpeed : MovementConfig.Speed;

        Movable.Move(GetMovementDirection(MovementInput), currentSpeed, MovementConfig.MovementSmoothness);
        Movable.ApplyGravity(MovementConfig.Gravity, MovementConfig.MaxFallSpeed);

        Rotatable.Rotate(RotationInput, RotationConfig.RotationSpeed);
    }

    private Vector3 GetMovementDirection(Vector2 movementVector)
    {
        Vector3 v = transform.right * movementVector.x + transform.forward * movementVector.y;
        v.Normalize();
        return v;
    }

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
}
