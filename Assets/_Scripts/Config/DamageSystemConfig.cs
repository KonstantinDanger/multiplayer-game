using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Config/Damage_System_")]
public class DamageSystemConfig : ScriptableObject
{
    [SerializeField, SubclassSelector] private List<DamageHandler> _damageHandlers = new();

    [field: SerializeField] public float BaseHp { get; private set; }
    public IReadOnlyList<DamageHandler> DamageHandlers => _damageHandlers;
}
