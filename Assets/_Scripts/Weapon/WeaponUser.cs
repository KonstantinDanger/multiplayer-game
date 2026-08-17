using System;
using UnityEngine;

public abstract class WeaponUser
{
    [field: SerializeField] public ScriptableWeapon InitialWeapon { get; private set; }

    private Transform _weaponHolderTransform;
    private MeshFilter _weaponMesh;
    private MeshRenderer _weaponRenderer;

    public abstract ScriptableWeapon SelectedWeapon { get; }

    public void Initialize(Transform weaponHolderTransform, MeshFilter weaponMesh, MeshRenderer weaponRenderer)
    {
        _weaponHolderTransform = weaponHolderTransform;
        _weaponMesh = weaponMesh;
        _weaponRenderer = weaponRenderer;

        if (InitialWeapon != null)
            Equip(InitialWeapon);
    }

    public abstract void Equip(ScriptableWeapon weapon);
    public abstract bool HasEquipped(ScriptableWeapon weapon);
    public abstract bool HasEquippedAny();
    public abstract void Unequip();

    protected void SetMesh(Mesh mesh, Material[] materials)
    {
        _weaponMesh.mesh = mesh;

        Material[] copiedMaterials = new Material[materials.Length];

        Array.Copy(materials, copiedMaterials, materials.Length);

        _weaponRenderer.materials = copiedMaterials;

        _weaponMesh.mesh.RecalculateBounds();
    }

    protected void ResetMesh()
    {
        _weaponMesh.mesh.Clear();
        _weaponRenderer.materials = new Material[] { };
    }
}
