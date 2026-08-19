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
    private Vector3 _cachedVelocity = Vector3.zero;

    public override Vector3 Modify(Projectile self, Vector3 velocity, float deltaTime)
    {
        if (!_cached)
        {
            _cachedVelocity = velocity;
            _cached = true;
        }

        float normalizedTime = Mathf.Clamp01(_elapsedTime / _moveTime);

        const float sampleOffset = 0.001f;

        float t1 = Mathf.Clamp01(normalizedTime - sampleOffset);
        float t2 = Mathf.Clamp01(normalizedTime + sampleOffset);

        float h1 = _verticalMovementCurve.Evaluate(t1);
        float h2 = _verticalMovementCurve.Evaluate(t2);

        // dh/dt in normalized curve units.
        float derivative = (h2 - h1) / (t2 - t1);

        //float verticalVelocityHeight = _verticalMovementCurve.Evaluate(normalizedTime) * _maxHeight;
        float verticalVelocityHeight = derivative * _maxHeight / _moveTime;

        velocity.y = verticalVelocityHeight;

        _elapsedTime += deltaTime;

        return velocity;
    }
}
