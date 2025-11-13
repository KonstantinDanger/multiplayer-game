using System.Collections.Generic;

[System.Serializable]
public class CharacterClass
{
    private readonly List<Ability> _abilities = new();

    public IReadOnlyList<Ability> Abilities => _abilities;

    public CharacterClass(List<Ability> abilities)
        => _abilities = abilities;
}

