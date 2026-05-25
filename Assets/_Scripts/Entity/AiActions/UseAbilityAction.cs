using Mirror;
using System;
using UnityEngine;

[Serializable]
public class UseAbilityAction : AIAction
{
    [SerializeField] private ScriptableAbility _ability;

    private IAbilityUser _abilityUser;
    private IRotatable _rotator;
    private Ability _abilityToUse;
    private Ability _runtimeSelectedAbility;

    public override void Initialize(NetworkBehaviour self)
    {
        _abilityUser = self.GetComponent<IAbilityUser>();
        _rotator = self.GetComponent<IRotatable>();

        _abilityToUse = _ability.GetNew();
        _abilityUser.Add(_abilityToUse);
    }

    public override void Execute(Enemy self, NetworkBehaviour target)
    {
        IsLocked = true;
        int abilityIndex = _abilityUser.FindIndexOf(_abilityToUse);
        //_runtimeSelectedAbility = _abilityUser.Use(_abilityToUse, target);
        _runtimeSelectedAbility = _abilityUser.Use(abilityIndex, target);

        Vector3 rotationDir = target.transform.position - self.transform.position;
        _rotator.Rotate(rotationDir, self.RotationConfig.RotationSpeed);

        if (_runtimeSelectedAbility == null)
        {
            IsLocked = false;
            return;
        }

        IsLocked = _runtimeSelectedAbility.IsPerforming;
    }
}
