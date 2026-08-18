using System;
using System.Collections.Generic;

[Serializable]
public class MultipleWeaponsUser : WeaponUser
{
    private readonly List<ScriptableWeapon> _weapons = new();

    private int _selectedIndex;

    public int WeaponsAmount => _weapons.Count;
    public override ScriptableWeapon SelectedWeapon => _weapons.Count == 0 ? null : _weapons[_selectedIndex];

    public void EquipNext()
    {
        if (_selectedIndex >= _weapons.Count - 1)
            _selectedIndex = 0;
        else
            _selectedIndex++;

        ResetMesh();
        SetMesh(SelectedWeapon.Mesh, SelectedWeapon.Materials);
    }

    public void EquipPrev()
    {
        if (_selectedIndex == 0)
            _selectedIndex = _weapons.Count - 1;
        else
            _selectedIndex--;

        ResetMesh();
        SetMesh(SelectedWeapon.Mesh, SelectedWeapon.Materials);
    }

    public override void Equip(ScriptableWeapon weapon)
    {
        if (_weapons.Contains(weapon))
            return;

        _weapons.Add(weapon);

        EquipNext();
    }

    public override bool HasEquipped(ScriptableWeapon weapon)
        => _weapons.Contains(weapon);

    public override void Unequip()
    {
        if (_weapons.Count == 0)
            return;

        _weapons.RemoveAt(_selectedIndex - 1);
        EquipPrev();
    }

    public override bool HasEquippedAny()
        => _weapons.Count > 0;
}
