using UnityEngine;

public interface IRotatablePlayerCamera : IRotatable
{
    public Transform Transform { get; }
    void HideCursor();
    void ShowCursor();
    void Initialize(bool isLocalPlayer);
    void RotateVertically(Vector3 direction, float speed);
}
