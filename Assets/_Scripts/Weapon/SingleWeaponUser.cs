using System;

[Serializable]
public class SingleWeaponUser : WeaponUser
{
    private ScriptableWeapon _selectedWeapon;

    public override ScriptableWeapon SelectedWeapon => _selectedWeapon;

    public override void Equip(ScriptableWeapon weapon)
    {
        if (HasEquipped(weapon))
            Unequip();

        _selectedWeapon = weapon;

        SetMesh(SelectedWeapon.Mesh, SelectedWeapon.Materials);
    }

    public override bool HasEquipped(ScriptableWeapon weapon)
        => SelectedWeapon == weapon;

    public override bool HasEquippedAny()
        => SelectedWeapon != null;

    public override void Unequip()
    {
        _selectedWeapon = null;
        ResetMesh();
    }
}
