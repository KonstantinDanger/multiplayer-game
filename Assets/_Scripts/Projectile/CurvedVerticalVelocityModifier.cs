using System;
using UnityEngine;

[Serializable]
public class CurvedVerticalVelocityModifier : ProjectileMovementVelocityModifier
{
    [SerializeField] private AnimationCurve _verticalMovementCurve;
    [SerializeField, Range(0f, 5f)] private float _maxHeight = 2f;
    [SerializeField, Range(0.001f, 10f)] private float _moveTime = 2f;

    private float _elapsedTime;

    private bool _cached;
    private Vector3 _senderVelocity = Vector3.zero;
    private Vector3 _baseVelocity;

    public override Vector3 Modify(Projectile self, Vector3 velocity, float deltaTime)
    {
        if (!_cached)
        {
            _baseVelocity = velocity;
            _baseVelocity.y = 0f;

            if (self.Data.Sender.TryGetComponent(out IMovable movable))
            {
                _senderVelocity = movable.Velocity;
                _senderVelocity.y = 0f;
            }

            _cached = true;
        }

        float normalizedTime = Mathf.Clamp01(_elapsedTime / _moveTime);

        const float sampleOffset = 0.001f;

        float t1 = Mathf.Clamp01(normalizedTime - sampleOffset);
        float t2 = Mathf.Clamp01(normalizedTime + sampleOffset);

        float h1 = _verticalMovementCurve.Evaluate(t1);
        float h2 = _verticalMovementCurve.Evaluate(t2);

        float derivative = (h2 - h1) / (t2 - t1);

        float verticalVelocity = derivative * _maxHeight / _moveTime;

        _elapsedTime += deltaTime;

        Vector3 vel = (_senderVelocity + _baseVelocity).normalized * _senderVelocity.magnitude;

        vel.y = verticalVelocity;

        return vel;
    }
}
