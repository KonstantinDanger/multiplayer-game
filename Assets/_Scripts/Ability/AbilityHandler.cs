using System;
using UnityEngine;

[Serializable]
public class AbilityHandler : Ability
{
    public Ability Ability { get; private set; }

    public override float CooldownTime => Ability.CooldownTime;
    public override string Name => Ability.Name;

    private float _nextUsageTime = 0;

    public AbilityHandler(Ability ability)
        => Ability = ability;

    protected override void OnPerform(GameObject sender, GameObject target)
    {
        if (!IsReadyToUse())
            return;

        Ability.Perform(sender, target);

        SetNextUseTime();
    }

    private bool IsReadyToUse()
        => Time.time >= _nextUsageTime;

    private void SetNextUseTime()
        => _nextUsageTime = Time.time + CooldownTime;
}
