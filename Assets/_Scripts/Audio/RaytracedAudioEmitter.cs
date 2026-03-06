using System;
using UnityEngine;

public class RaytracedAudioEmitter : MonoBehaviour
{
    /// <summary>
    /// Speed of sound in meters per second (m/s)
    /// </summary>
    private const int SPEED_OF_SOUND = 343;

    private struct RayData
    {
        public int Reflections;
        public float Distance;
        public bool ReachedToTarget;
        public Vector3 LastHitPoint;
        public RayTracedAudioSource AudioSource;
    }

    [SerializeField, Range(0f, 1f)] private float _updatePeriod = 0.2f;
    [SerializeField, Range(0f, 100f)] private float _maxRayDistance = 50f;
    [SerializeField, Range(0f, 2f)] private float _audioDetectionRadius = 0.5f;
    [SerializeField, Range(0, 1000)] private int _maxRayCount = 100;
    [SerializeField, Range(0, 100)] private int _maxBounces = 10;
    [SerializeField] private LayerMask _surfaceLayers;

    private float _nextEmitTime;

    private RaytracedAudioSources _sources;

    private void Start()
        => _sources = ServiceLocator.Container.Resolve<RaytracedAudioSources>();

    private void Update()
    {
        if (Time.time < _nextEmitTime)
            return;

        EmitRays();
        _nextEmitTime = Time.time + _updatePeriod;
    }

    public void EmitRays()
    {
        RayTracedAudioSource source = null;
        RaytracedAudioParams audioParams = new();
        audioParams.Delay = float.MaxValue;
        int hits = 0;

        for (int i = 0; i < _maxRayCount; i++)
        {
            RayData data = CalculateRay(transform.position, UnityEngine.Random.onUnitSphere);
            source = data.AudioSource;

            if (!data.ReachedToTarget)
                continue;

            hits++;

            RaytracedAudioParams raytracedData = CalculateRayTracedParams(data);

            audioParams.Energy += raytracedData.Energy;
            audioParams.Occlusion += raytracedData.Occlusion;
            audioParams.Reverb += raytracedData.Reverb * raytracedData.Energy;
            audioParams.Delay = Mathf.Min(audioParams.Delay, raytracedData.Delay);
        }

        if (hits == 0)
            return;

        audioParams.Occlusion /= hits;
        audioParams.Energy = Mathf.Clamp01(audioParams.Energy);
        audioParams.Reverb /= audioParams.Energy;

        if (!source)
            return;

        source.Setup(audioParams);
    }

    private RaytracedAudioParams CalculateRayTracedParams(RayData data)
    {
        float occlusion = CalculateOcclusion();
        float energy = CalculateEnergy(data.Distance);
        float delay = CalculateDelay(data.Distance);
        float reverb = CalculateReverb(energy, data.Reflections);

        var raytracedData = new RaytracedAudioParams
        {
            Occlusion = occlusion,
            Energy = energy,
            Delay = delay,
            Reverb = reverb,
        };

        return raytracedData;
    }

    private RayData CalculateRay(Vector3 position, Vector3 direction)
    {
        RayData data = new RayData();
        Vector3 currentPosition = position;
        Vector3 currentDirection = direction;

        for (int bounce = 0; bounce < _maxBounces; bounce++)
        {
            if (!Physics.Raycast(currentPosition, currentDirection, out RaycastHit hit, _maxRayDistance, _surfaceLayers, QueryTriggerInteraction.Ignore))
                break;

            float distance = (hit.point - currentPosition).magnitude;

            data.LastHitPoint = hit.point;
            data.Reflections++;
            data.Distance += distance;

            foreach (RayTracedAudioSource src in _sources.Sources)
            {
                Vector3 closest = ClosestPointToSource(currentPosition, currentDirection, src.transform.position);
                if ((closest - src.transform.position).magnitude < _audioDetectionRadius)
                {
                    data.AudioSource = src;
                    data.ReachedToTarget = true;
                    return data;
                }
            }

            currentDirection = Vector3.Reflect(currentDirection, hit.normal);
            currentPosition = hit.point + currentDirection * 0.01f;
        }

        return data;
    }

    private Vector3 ClosestPointToSource(Vector3 a, Vector3 b, Vector3 p)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(p - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return a + ab * t;
    }

    private float CalculateReverb(float energy, int reflections) => energy * (reflections / (float)_maxBounces);
    private float CalculateDelay(float distance) => distance / SPEED_OF_SOUND;
    private float CalculateEnergy(float distance) => 1f / Mathf.Max(distance * distance, 1f);
    private float CalculateOcclusion() => 1;
}

