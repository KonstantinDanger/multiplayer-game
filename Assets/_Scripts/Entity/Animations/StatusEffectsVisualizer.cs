using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectsVisualizer : NetworkBehaviour
{
    [SerializeField] private StatusEffectHandler _effectHandler;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private ParticleSystemShapeType _shapeType;

    private readonly Dictionary<IStatusEffectPresentationData, ParticleSystem> _instances = new();

    protected override void OnValidate()
    {
        base.OnValidate();

        if (_shapeType != ParticleSystemShapeType.SkinnedMeshRenderer && _shapeType != ParticleSystemShapeType.MeshRenderer)
        {
            Debug.LogError("shape type only allowed to be skinned or mesh renderers!");
            _shapeType = ParticleSystemShapeType.SkinnedMeshRenderer;
        }
    }

    private void OnEnable()
    {
        _effectHandler.OnInstantEffectProc += HandleInstantEffectProc;
        _effectHandler.OnProlongedEffectProc += HandleProlongedEffectProc;
        _effectHandler.OnProlongedEffectRemoved += HandleProlongedEffectRemoved;
    }

    private void OnDisable()
    {
        _effectHandler.OnInstantEffectProc -= HandleInstantEffectProc;
        _effectHandler.OnProlongedEffectProc -= HandleProlongedEffectProc;
        _effectHandler.OnProlongedEffectRemoved -= HandleProlongedEffectRemoved;
    }

    private void HandleProlongedEffectRemoved(IStatusEffectPresentationData data)
    {
        if (_instances.TryGetValue(data, out ParticleSystem instance))
        {
            RemoveParticle(instance, data);
        }
    }

    private void HandleProlongedEffectProc(IStatusEffectPresentationData data)
    {
        if (_instances.ContainsKey(data))
            return;

        _instances.Add(data, CreateParticle(data.ProlongedStatusParticle));
    }

    private void HandleInstantEffectProc(IStatusEffectPresentationData data)
        => RemoveParticle(CreateParticle(data.ProcParticle), data);

    private ParticleSystem CreateParticle(ParticleSystem particleSystem)
    {
        var particle = Instantiate(particleSystem, transform);
        var shape = particle.shape;

        shape.shapeType = _shapeType;

        shape.skinnedMeshRenderer = _skinnedMeshRenderer;
        shape.meshRenderer = _meshRenderer;

        shape.meshShapeType = ParticleSystemMeshShapeType.Edge;

        particle.Play();

        return particle;
    }

    private void RemoveParticle(ParticleSystem particle, IStatusEffectPresentationData data)
    {
        particle.Stop();
        Destroy(particle.gameObject, particle.main.duration);
        _instances.Remove(data);
    }
}


