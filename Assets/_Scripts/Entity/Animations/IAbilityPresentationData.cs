using UnityEngine;

public interface IAbilityPresentationData
{
    Sprite SpriteIcon { get; }
    AnimationClip PreparationAnimation { get; }
    AnimationClip UsageAnimation { get; }
}
