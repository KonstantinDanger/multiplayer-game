using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Ability")]
public class ScriptableAbility : ScriptableObject, IAbilityPresentationData
{
    [field: SerializeField] public virtual string Name { get; private set; }
    [SerializeReference, SubclassSelector] private Ability _ability;

    [field: Header("UI")]
    [field: SerializeField] public Sprite SpriteIcon { get; private set; }

    [field: Header("Animations")]
    [field: SerializeField] public virtual AnimationClip PreparationAnimation { get; private set; }
    [field: SerializeField] public virtual AnimationClip UsageAnimation { get; private set; }
    //[field: Header("VFX")]
    //[field: Header("SFX")]

    public Ability GetNew()
        => Utils.GetInstancedCopyOf(_ability);

    public Type GetAbilityType()
        => _ability.GetType();

    public override bool Equals(object obj)
    {
        if (obj is not ScriptableAbility other)
            return false;

        if (PreparationAnimation == null || UsageAnimation == null)
            return base.Equals(obj);

        return other.Equals(obj)
            && EqualityComparer<AnimationClip>.Default.Equals(other.PreparationAnimation, PreparationAnimation)
            && EqualityComparer<AnimationClip>.Default.Equals(UsageAnimation, UsageAnimation);
    }

    public override int GetHashCode()
        => base.GetHashCode();
}
