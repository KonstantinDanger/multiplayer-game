using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Damage_System_")]
public class DamageSystemConfig : ScriptableObject
{
    [field: SerializeField] public StatParameter BaseHealth { get; private set; } = new(StatType.Health, 100);
    [field: SerializeField, Range(1, 30)] public float RespawnTime { get; private set; } = 5;

    [Space()]
    [SerializeReference, SubclassSelector] private List<DamageHandler> _damageHandlers = new();

    public IReadOnlyList<DamageHandler> DamageHandlers => _damageHandlers;
}
