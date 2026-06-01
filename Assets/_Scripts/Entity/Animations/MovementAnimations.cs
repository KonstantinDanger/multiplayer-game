using AYellowpaper;
using Mirror;
using UnityEngine;

public class MovementAnimations : NetworkBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private InterfaceReference<IMovable> _movable;
    [SerializeField] private InterfaceReference<IRotatable> _rotatable;
    [SerializeField] private MovementConfig _config;
    [SerializeField] private float _rotationSmoothness = 0.03f;
    [SerializeField] private float _speedDeadZoneThreshold = 0.1f;

    [SerializeField] private bool _serverSide = true;

    [Header("Params")]
    [SerializeField] private string _verticalInputParamName = "VerticalVelocity";
    [SerializeField] private string _horizontalInputParamName = "HorizontalVelocity";
    [SerializeField] private string _movementAngle = "MovementAngle";
    [SerializeField] private string _airborneVelocityParamName = "AirborneVelocity";
    [SerializeField] private string _isGroundedParamName = "IsGrounded";


    private IMovable Movable => _movable.Value;
    private IRotatable Rotatable => _rotatable.Value;

    private float _verticalVelocity;
    private float _horizontalVelocity;
    private float _rotationAngleDelta;

    private void OnEnable()
    {
        Movable.OnMove += HandleMove;
        Rotatable.OnRotate += HandleRotation;
    }

    private void OnDisable()
    {
        Movable.OnMove -= HandleMove;

        Rotatable.OnRotate -= HandleRotation;
    }

    private void HandleRotation(float rotationAngle)
    {
        //float targetRotation = rotationAngle * 360;
        //_rotationAngleDelta = Mathf.Lerp(_rotationAngleDelta, targetRotation, _rotationSmoothness);

        //_anim.SetFloat(_movementAngle, _rotationAngleDelta * Time.deltaTime);
    }

    private void HandleMove()
    {
        bool isOffline = !NetworkClient.active && !NetworkServer.active;
        bool hasAuthority = isServer || isLocalPlayer || isOffline;

        if (!hasAuthority)
            return;

        bool isGrounded = Movable.Velocity.y < 0.1f;

        if (isGrounded)
            HandleGroundedMovement();

        _anim.SetBool(_isGroundedParamName, isGrounded);
    }

    private void HandleAirborneMovement()
    {
        //float velocityY = Movable.Velocity.y;
        //velocityY = Mathf.Clamp(velocityY, -1, 1);
        //_anim.SetFloat(_airborneVelocityParamName, velocityY);
    }

    private void HandleGroundedMovement()
    {
        Vector3 velocity = Movable.Velocity;
        velocity.y = 0f;
        velocity = Vector3.ClampMagnitude(velocity, 1f);

        velocity = transform.InverseTransformDirection(velocity);

        float speedMultiplier = Movable.Velocity.magnitude / _config.SprintSpeed;

        if (speedMultiplier <= _speedDeadZoneThreshold)
            speedMultiplier = 0f;

        _verticalVelocity = velocity.z * speedMultiplier;
        _horizontalVelocity = velocity.x * speedMultiplier;

        if (velocity.magnitude < 0.05f)
        {
            _verticalVelocity = 0f;
            _horizontalVelocity = 0f;
        }

        _anim.SetFloat(_verticalInputParamName, _verticalVelocity);
        _anim.SetFloat(_horizontalInputParamName, _horizontalVelocity);
    }
}
