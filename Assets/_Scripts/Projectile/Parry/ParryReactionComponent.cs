using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Projectile))]
public class ParryReactionComponent : MonoBehaviour, IParryable
{
    [SerializeReference, SubclassSelector] private ProjectileParryReaction _parryReaction;

    public void ReactTo(GameObject parrySender)
        => _parryReaction.ReactTo(parrySender);
}
