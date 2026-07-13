using System;

[Serializable]
public class StunEffect : ProlongedStatusEffect
{
    // duration
    // incoming damage multiplier

    // stuns for a prolonged time
    // decreases defense
    protected override void OnProc(Entity entity) => UnityEngine.Debug.Log("Stun has been applied ");
    protected override void OnTick(float deltaTime) => UnityEngine.Debug.Log("Stun tick ");
}


