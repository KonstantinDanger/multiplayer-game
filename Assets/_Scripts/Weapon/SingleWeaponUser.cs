using System;
using UnityEngine;

public class SingleWeaponUser : WeaponUser
{
    [SerializeField] private Transform _weaponHolderTransform;
    [SerializeField] private MeshFilter _weaponMesh;
    [SerializeField] private MeshRenderer _weaponRenderer;

    private ScriptableWeapon _weapon;

    public override void Equip(ScriptableWeapon weapon)
    {
        if (HasEquipped())
            Unequip();

        _weapon = weapon;
        Mesh mesh = weapon.Mesh;
        mesh.bounds = new Bounds(_weaponHolderTransform.localPosition, mesh.bounds.size);
        _weaponMesh.mesh = weapon.Mesh;

        Material[] materials = new Material[weapon.Materials.Length];

        Array.Copy(weapon.Materials, materials, weapon.Materials.Length);

        _weaponRenderer.materials = materials;

        _weaponMesh.mesh.RecalculateBounds();
    }

    public override bool HasEquipped()
        => _weapon != null;

    public override void Unequip()
    {
        _weapon = null;
        _weaponMesh.mesh.Clear();
        _weaponRenderer.materials = null;
    }
}
