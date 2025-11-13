using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Class/Class")]
public class ScriptableCharacterClass : ScriptableObject
{
    [SerializeField] private List<ScriptableAbility> _abilities = new();

    public CharacterClass GetNew()
    {
        var abilities = _abilities
            .Select(a => a.GetNew())
            .ToList();

        return new(abilities);
    }
}

