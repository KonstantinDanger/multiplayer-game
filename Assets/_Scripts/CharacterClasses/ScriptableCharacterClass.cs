using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Class/Class")]
public class ScriptableCharacterClass : ScriptableObject
{
    [field: SerializeField] public string ClassName { get; private set; }
    [field: SerializeField] public Sprite PreviewSprite { get; private set; }
    [field: SerializeField] public ParallelAbilityExecutionMatrix AbilityExecutionMatrix { get; private set; }

    [SerializeField] private List<ScriptableAbility> _abilities = new();

    public CharacterClass GetNew()
    {
        var abilities = _abilities
            .Select(a => a.GetNew())
            .ToList();

        return new(abilities);
    }
}

