using AYellowpaper;
using UnityEngine;

public class MovementTrail : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trail;
    [SerializeField] private InterfaceReference<IMovable> _movable;
    [SerializeField, Range(0f, 360f)] private float _velocityAngleToRenderTrail = 180f;

    private IMovable Movable => _movable.Value;

    private void OnEnable()
        => Movable.OnMove += HandleMove;

    private void OnDisable()
        => Movable.OnMove -= HandleMove;

    private void HandleMove()
    {
        bool render = Vector3.Angle(transform.forward, Movable.Velocity) < _velocityAngleToRenderTrail / 2f;
        _trail.enabled = render;
    }
}
