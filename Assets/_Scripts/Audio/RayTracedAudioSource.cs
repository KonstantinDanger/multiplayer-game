using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
[RequireComponent(typeof(AudioReverbFilter))]
public class RayTracedAudioSource : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioLowPassFilter _lowPassFilter;
    [SerializeField] private AudioReverbFilter _reverb;

    private RaytracedAudioSources _sources;

    private void OnValidate()
    {
        if (!_source)
            _source = GetComponent<AudioSource>();

        if (!_lowPassFilter)
            _lowPassFilter = GetComponent<AudioLowPassFilter>();

        if (!_reverb)
            _reverb = GetComponent<AudioReverbFilter>();
    }

    private void Awake()
        => _sources = ServiceLocator.Container.Resolve<RaytracedAudioSources>();

    private void OnEnable()
        => _sources.Add(this);

    private void OnDisable()
        => _sources.Remove(this);

    public void Setup(RaytracedAudioParams parameters)
    {
        _source.volume = parameters.Energy;
        _lowPassFilter.cutoffFrequency = Mathf.Lerp(22000f, 500f, parameters.Occlusion);
        _source.PlayDelayed(parameters.Delay);
        _reverb.reverbLevel = Mathf.Lerp(-10000f, 0, parameters.Reverb);
    }
}

