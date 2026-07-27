using Mirror;

public abstract class WeaponUser : NetworkBehaviour
{
    public abstract void Equip(ScriptableWeapon weapon);
    public abstract bool HasEquipped();
    public abstract void Unequip();
}
