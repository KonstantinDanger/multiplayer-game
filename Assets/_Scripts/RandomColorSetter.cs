using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class RandomColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer _renderer;

    [SyncVar(hook = nameof(OnColorChanged))] private Color _color;

    private static readonly Dictionary<int, Color> _randomColors = new()
    {
        [0] = Color.white,
        [1] = Color.black,
        [2] = Color.blue,
        [3] = Color.red,
        [4] = Color.gray,
        [5] = Color.cyan,
        [6] = Color.green,
        [7] = Color.yellow,
        [8] = Color.magenta,
    };

    private void Start()
    {
        if (isServer)
        {
            _color = GetRandomColor();
        }

        OnColorChanged(_renderer.material.color, _color);
    }

    private void OnColorChanged(Color old, Color newColor)
        => _renderer.material.color = newColor;

    private Color GetRandomColor()
        => _randomColors[Random.Range(0, 9)];
}
