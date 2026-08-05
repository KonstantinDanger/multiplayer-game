using UnityEngine;

public interface IAbilityPresentationData
{
    string Name { get; }
    Sprite SpriteIcon { get; }
    AnimationClip PreparationAnimation { get; }
    AnimationClip UsageAnimation { get; }
    bool ConsiderAbilityDuration { get; }
}
