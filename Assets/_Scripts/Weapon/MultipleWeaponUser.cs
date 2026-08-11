using System.Collections.Generic;

public class MultipleWeaponUser : WeaponUser
{
    private List<ScriptableWeapon> _weapons = new();

    private int _selectedIndex;

    private ScriptableWeapon SelectedWeapon => _weapons.Count == 0 ? null : _weapons[_selectedIndex - 1];

    public void EquipNext()
    {
        if (_selectedIndex >= _weapons.Count - 1)
            _selectedIndex = 0;
        else
            _selectedIndex++;
        // Update mesh
    }

    public void EquipPrev()
    {
        if (_selectedIndex == 0)
            _selectedIndex = _weapons.Count - 1;
        else
            _selectedIndex--;

        // Update mesh
    }

    public override void Equip(ScriptableWeapon weapon)
    {
        if (_weapons.Contains(weapon))
            return;

        _weapons.Add(weapon);

        // Assign mesh

        _selectedIndex++;
    }

    public override bool HasEquipped() => SelectedWeapon != null;

    public override void Unequip()
    {
        if (_weapons.Count == 0)
            return;

        _weapons.RemoveAt(_selectedIndex - 1);
        _selectedIndex--;
    }
}
