using UnityEngine;

public class WeaponUserComponent : MonoBehaviour
{
    [SerializeField] private Transform _weaponHolderTransform;
    [SerializeField] private MeshFilter _weaponMesh;
    [SerializeField] private MeshRenderer _weaponRenderer;

    public WeaponUser WeaponUser { get; private set; }

    public void Initialize(WeaponUser weaponUser)
    {
        WeaponUser = weaponUser;
        WeaponUser?.Initialize(_weaponHolderTransform, _weaponMesh, _weaponRenderer);
    }
}
