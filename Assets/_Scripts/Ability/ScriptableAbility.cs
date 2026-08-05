using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/Ability")]
public class ScriptableAbility : ScriptableObject, IAbilityPresentationData
{
    [field: SerializeField] public string Name { get; private set; }
    [SerializeReference, SubclassSelector] private Ability _ability;

    [field: Header("UI")]
    [field: SerializeField] public Sprite SpriteIcon { get; private set; }

    [field: Header("Animations")]
    [field: SerializeField] public bool ConsiderAbilityDuration { get; private set; }
    [field: SerializeField] public AnimationClip PreparationAnimation { get; private set; }
    [field: SerializeField] public AnimationClip UsageAnimation { get; private set; }
    //[field: Header("VFX")]
    //[field: Header("SFX")]

    public Ability GetNew()
        => Utils.GetInstancedCopyOf(_ability);

    public Type GetAbilityType()
        => _ability.GetType();
}
