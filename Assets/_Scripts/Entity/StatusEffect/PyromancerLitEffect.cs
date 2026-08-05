using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PyromancerLitEffect : ProlongedStatusEffect
{
    [SerializeField, Range(0f, 500f)] private float _damagePercentBuff = 15f;
    [SerializeField] private List<ScriptableAbility> _buffableAbilities = new();

    private IAbilityUser _abilityUser;

    private bool _succeed;

    public override void OnValidate()
    {
        if (_buffableAbilities.Count == 0)
            return;

        foreach (ScriptableAbility ability in _buffableAbilities.ToList())
        {
            if (ability == null)
                continue;

            if (ability.GetAbilityType() == typeof(ComboAbility))
            {
                UnityEngine.Debug.LogError("Combo abilities are not supported for this effect. Consider adding each attack ability from the combo individually");
                _buffableAbilities.Remove(ability);
                continue;
            }

            if (!typeof(AttackAbility).IsAssignableFrom(ability.GetAbilityType()))
            {
                string warning = $"The ability \"{ability.name}\" is not an attack ability. Removing it from list...";

                UnityEngine.Debug.LogError(warning);
                _buffableAbilities.Remove(ability);
            }
        }
    }

    protected override void OnProc(GameObject target)
        => target.TryGetComponent(out _abilityUser);

    protected override void OnTick(float deltaTime)
    {
        if (_abilityUser == null)
            return;

        if (!Active)
            return;

        if (_succeed)
            return;

        TryBuffNextAbility();
    }

    private void TryBuffNextAbility()
    {
        AbilityInstance nextUsedAbility = _abilityUser.AbilityInstances
            .Where(instance => _buffableAbilities.Any(ability => instance.Contains(ability)))
            .FirstOrDefault(instance => instance.ability.UseTime > ExpirationTime - Duration && instance.ability.UseTime < ExpirationTime);

        if (nextUsedAbility == null)
            return;

        UnityEngine.Debug.Log("used ability " + nextUsedAbility.presentationData.Name);

        float damage = (nextUsedAbility.ability as AttackAbility).DamageAmount;
        //We need to somehow buff attack from selected ability only once per attack cycle and then reset to default!
        UnityEngine.Debug.Log("Base damage: " + damage + " | Amplified damage: " + (damage + damage * (_damagePercentBuff / 100f)));
        _succeed = true;
    }

    protected override void OnReset()
    {
        base.OnReset();

        _succeed = false;
    }
}
