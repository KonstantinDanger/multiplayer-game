using System;
using UnityEngine;

[Serializable]
public class TransitionMatrix
{
    [SerializeField]
    private bool[] values;

    public bool Get(int row, int column, int size)
        => values[row * size + column];

    public void Set(int row, int column, int size, bool value)
        => values[row * size + column] = value;

    public void Resize(int size)
    {
        int newLength = size * size;

        if (values == null)
        {
            values = new bool[newLength];
            return;
        }

        Array.Resize(ref values, newLength);
    }
}
