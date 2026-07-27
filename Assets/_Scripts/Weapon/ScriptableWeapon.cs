using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableWeapon/Weapon", fileName = "Weapon_")]
public class ScriptableWeapon : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Mesh Mesh { get; private set; }
    [field: SerializeField] public Material[] Materials { get; private set; }
    [field: SerializeField] public bool Permanent { get; private set; }
    [field: SerializeField, Range(0f, 120f)] public float Lifetime { get; private set; }
    [field: SerializeField, Range(0f, 10000f)] public float Durability { get; private set; }
}