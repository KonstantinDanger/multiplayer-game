using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectView : UIView
{
    [SerializeField] private CharacterSelectCell _cellPrefab;
    [SerializeField] private Transform _gridContent;

    private Player _player;
    private CharacterClassList _classes;
    private readonly List<CharacterSelectCell> _cells = new();

    public void Initialize(CharacterClassList list, Player player)
    {
        _classes = list;
        _player = player;

        Clear();
        DrawClasses(_classes);
    }

    private void DrawClasses(CharacterClassList list)
    {
        var classes = list.Classes;

        foreach (var c in classes)
        {
            var cell = Instantiate(_cellPrefab, _gridContent);
            cell.Initialize(c.Key, c.Value, _player);
            _cells.Add(cell);
        }
    }

    private void Clear()
    {
        foreach (var c in _cells)
        {
            Destroy(c.gameObject);
        }
    }
}
