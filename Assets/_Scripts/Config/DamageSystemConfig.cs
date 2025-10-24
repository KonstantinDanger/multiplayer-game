using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Damage_System_")]
public class DamageSystemConfig : ScriptableObject
{
    [field: SerializeField, Range(1, 1000)] public float BaseHp { get; private set; } = 100;
    [field: SerializeField, Range(1, 30)] public float RespawnTime { get; private set; } = 5;

    [Space()]
    [SerializeReference, SubclassSelector] private List<DamageHandler> _damageHandlers = new();

    public IReadOnlyList<DamageHandler> DamageHandlers => _damageHandlers;
}
