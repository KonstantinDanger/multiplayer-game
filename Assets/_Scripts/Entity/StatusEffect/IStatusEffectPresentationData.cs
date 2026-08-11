using UnityEngine;

public interface IStatusEffectPresentationData
{
    ParticleSystem ProcParticle { get; }
    ParticleSystem ProlongedStatusParticle { get; }
}