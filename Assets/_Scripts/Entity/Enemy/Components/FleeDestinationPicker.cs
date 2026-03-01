using UnityEngine;

public class FleeDestinationPicker
{
    private float _timeOffset;
    private float _noiseOffset;
    private bool _initialized = false;

    private readonly FleeConfig _config;

    public FleeDestinationPicker(FleeConfig config)
        => _config = config;

    public Vector3 GetFleeDestinationFrom(Transform attacker, Transform self, float fleeDistance)
    {
        if (!_initialized)
        {
            _timeOffset = UnityEngine.Random.Range(0f, 100f);
            _noiseOffset = UnityEngine.Random.Range(0f, 100f);
            _initialized = true;
        }

        Vector3 awayFromTarget = (self.position - attacker.position).normalized;

        float sineValue = Mathf.Sin((Time.time + _timeOffset) * _config.SineFrequency);
        float sineAngle = sineValue * _config.ChaoticAngleVariation;

        float noise1 = Mathf.PerlinNoise((Time.time + _noiseOffset) * 0.8f, _noiseOffset);
        float noise2 = Mathf.PerlinNoise(_noiseOffset, (Time.time + _noiseOffset) * 1.2f);
        float noiseAngle = (noise1 - 0.5f) * _config.NoiseAmount + (noise2 - 0.5f) * _config.NoiseAmount * 0.5f;

        float finalAngle = sineAngle + noiseAngle;

        Quaternion rotation = Quaternion.Euler(0, finalAngle, 0);
        Vector3 fleeDirection = rotation * awayFromTarget;

        Vector3 targetPosition = self.position + fleeDirection * fleeDistance;

        return targetPosition;
    }
}