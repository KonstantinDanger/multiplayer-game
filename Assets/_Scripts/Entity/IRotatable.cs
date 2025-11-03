using UnityEngine;

public interface IRotatable
{
    /// <summary>
    /// Rotation angle param
    /// </summary>
    event System.Action<float> OnRotate;

    void Rotate(Vector3 direction, float speed);
}
