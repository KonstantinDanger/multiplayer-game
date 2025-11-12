using System;
using UnityEngine;

[Serializable]
public class AbilityHandler : Ability
{
    [field: SerializeField] public Ability Ability;

    public override float CooldownTime => Ability.CooldownTime;
    public override string Name => Ability.Name;

    private float _nextUsageTime = 0;

    public AbilityHandler(string name) : base(name) { }

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
