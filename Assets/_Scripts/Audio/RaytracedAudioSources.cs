using System.Collections.Generic;

public class RaytracedAudioSources
{
    private readonly List<RayTracedAudioSource> _sources = new();

    public IEnumerable<RayTracedAudioSource> Sources => _sources;

    public void Add(RayTracedAudioSource source)
    {
        if (_sources.Contains(source))
            return;

        _sources.Add(source);
    }

    public void Remove(RayTracedAudioSource source)
        => _sources.Remove(source);
}

