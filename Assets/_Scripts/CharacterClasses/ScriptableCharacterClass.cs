using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Class/Class")]
public class ScriptableCharacterClass : ScriptableObject
{
    [field: SerializeField] public string ClassName { get; private set; }
    [field: SerializeField] public Sprite PreviewSprite { get; private set; }
    [field: SerializeField] public ParallelAbilityExecutionMatrix AbilityExecutionMatrix { get; private set; }

    [SerializeField] private List<ScriptableAbility> _abilities = new();

    [SerializeReference, SubclassSelector] private WeaponUser _weaponUser;

    public CharacterClass GetNew()
    {
        List<ScriptableAbility> abilities = new(_abilities);

        return new(abilities);
    }

    public WeaponUser GetWeaponUser()
        => _weaponUser == null ? null : Utils.GetInstancedCopyOf(_weaponUser);
}

