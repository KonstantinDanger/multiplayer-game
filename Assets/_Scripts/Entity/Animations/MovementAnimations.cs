using AYellowpaper;
using Mirror;
using UnityEngine;

public class MovementAnimations : NetworkBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private InterfaceReference<IMovable> _movable;
    [SerializeField] private InterfaceReference<IRotatable> _rotatable;
    [SerializeField] private MovementConfig _config;

    [Header("Params")]
    [SerializeField] private string _verticalInputParamName = "VerticalInput";
    [SerializeField] private string _horizontalInputParamName = "HorizontalInput";


    private IMovable Movable => _movable.Value;
    private IRotatable Rotatable => _rotatable.Value;

    private void OnEnable()
        => Movable.OnMove += HandleMove;

    private void OnDisable()
        => Movable.OnMove -= HandleMove;

    private void HandleMove()
    {
        Vector3 velocity = Movable.Velocity;
        velocity.y = 0f;
        velocity = Vector3.ClampMagnitude(velocity, 1f);

        velocity = transform.InverseTransformDirection(velocity);

        float speedMultiplier = Movable.Velocity.magnitude / _config.SprintSpeed;

        float vertical = velocity.z * speedMultiplier;
        float horizontal = velocity.x * speedMultiplier;

        if (velocity.magnitude < 0.05f)
        {
            vertical = 0f;
            horizontal = 0f;
        }

        _anim.SetFloat(Animator.StringToHash(_verticalInputParamName), vertical);
        _anim.SetFloat(Animator.StringToHash(_horizontalInputParamName), horizontal);
    }
}
