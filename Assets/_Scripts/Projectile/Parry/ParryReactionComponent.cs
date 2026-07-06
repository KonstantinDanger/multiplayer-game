using Mirror;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Projectile))]
public class ParryReactionComponent : MonoBehaviour, IParryable
{
    [SerializeReference, SubclassSelector] private ProjectileParryReaction _parryReaction;

    private bool _parried;

    public void ReactTo(NetworkBehaviour parrySender)
    {
        if (_parried)
            return;

        _parryReaction.ReactTo(parrySender);

        _parried = true;
    }
}
