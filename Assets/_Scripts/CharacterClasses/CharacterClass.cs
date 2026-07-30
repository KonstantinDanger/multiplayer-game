using System.Collections.Generic;

[System.Serializable]
public class CharacterClass
{
    private readonly List<ScriptableAbility> _abilities = new();

    public IReadOnlyList<ScriptableAbility> Abilities => _abilities;

    public CharacterClass(List<ScriptableAbility> abilities)
        => _abilities = abilities;
}

