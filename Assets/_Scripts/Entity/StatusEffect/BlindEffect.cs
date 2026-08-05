using Mirror;
using UnityEngine;

[System.Serializable]
public class BlindEffect : ProlongedStatusEffect
{
    private NetworkBehaviour _entitysTarget;
    private TargetTrackingMemory _entitysMemory;

    protected override void OnProc(GameObject target)
    {
        if (target.TryGetComponent(out Player player))
        {
            if (player.TryGetComponent(out BlindScreenEffectPlayer blindEffect))
            {
                blindEffect.Blind(Duration);
            }
        }
        else if (target.TryGetComponent(out _entitysMemory))
        {
            _entitysTarget = _entitysMemory.Target;

            _entitysMemory.ForgetTarget();
        }
        else
        {
            UnityEngine.Debug.Log("No player or enemy found ");
        }
    }

    protected override void OnReset()
    {
        if (_entitysMemory == null || _entitysTarget == null)
            return;

        _entitysMemory.Memorize(_entitysTarget);
    }
}