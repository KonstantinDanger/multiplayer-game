using Mirror;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class CombatStrafeAction : AIAction
{
    [Header("Note, that min time should be lesser then max!")]
    [SerializeField, Min(0.1f)] private float _minTimeToChangeDirection = 2f;
    [SerializeField, Min(0.1f)] private float _maxTimeToChangeDirection = 2f;

    private Transform _selfTransform;
    private IMovable _movable;
    private IRotatable _rotatable;

    private float _nextChangeDirectionTime;
    private bool _rightDirection;

    public override void Initialize(NetworkBehaviour self)
    {
        if (!self.TryGetComponent(out _movable))
            throw new Exception("No IMovable found!");

        if (!self.TryGetComponent(out _rotatable))
            throw new Exception("No IRotatable found!");

        _selfTransform = self.transform;

        SetNextDirectionChangeTime();
        SetRandomDirection();

        IsLocked = true;
    }

    public override void Execute(Enemy self, NetworkBehaviour target)
    {
        if (Time.time >= _nextChangeDirectionTime)
        {
            SetNextDirectionChangeTime();
            SetRandomDirection();
        }

        //if (target == null)
        //    target = GameObject.FindObjectOfType(typeof(Player), true) as Player;

        //_rotatable.Rotate(target.transform.position - self.transform.position, self.RotationConfig.RotationSpeed);
        Strafe(GetStrafeDirection(), self.MovementConfig.StrafeSpeed);
    }

    private void Strafe(Vector3 direction, float speed)
        => _movable.Move(direction, speed);

    private Vector3 GetStrafeDirection()
    {
        Vector3 direction = _rightDirection ? _selfTransform.right : -_selfTransform.right;
        direction.y = 0f;
        direction.Normalize();
        return direction;
    }

    private void SetNextDirectionChangeTime()
    {
        float time = Random.Range(_minTimeToChangeDirection, _maxTimeToChangeDirection);
        _nextChangeDirectionTime = Time.time + time;
    }

    private void SetRandomDirection()
    {
        int dir = Random.Range(0, 2);
        _rightDirection = dir == 0;
    }
}
