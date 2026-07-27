using System;

public class MultipleWeaponUser : WeaponUser
{
    public override void Equip(ScriptableWeapon weapon) => throw new NotImplementedException();
    public override bool HasEquipped() => throw new NotImplementedException();
    public override void Unequip() => throw new NotImplementedException();
}
