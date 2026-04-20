using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Class/Class list")]
public class CharacterClassList : ScriptableObject
{
    [SerializeField] private SerializedDictionary<ScriptableCharacterClass, Currency> _classes = new();

    public IReadOnlyDictionary<ScriptableCharacterClass, Currency> Classes => _classes;
}

