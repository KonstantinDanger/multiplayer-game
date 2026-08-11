using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffect/StatusEffect", fileName = "StatusEffect_")]
public class ScriptableStatusEffect : ScriptableObject, IStatusEffectPresentationData
{
    [SerializeReference, SubclassSelector] private StatusEffect _statusEffect;

    [field: SerializeField] public ParticleSystem ProcParticle { get; private set; }
    [field: SerializeField] public ParticleSystem ProlongedStatusParticle { get; private set; }

    private void OnValidate()
        => _statusEffect.OnValidate();

    public StatusEffect GetNew()
        => Utils.GetInstancedCopyOf(_statusEffect);
}


