using System;
using UnityEngine;

public interface IInputBrain
{
    void Update();
    void OnEnable();
    void OnDisable();

    Vector2 MovementVector { get; }
    Vector2 Rotation { get; }
    bool IsSprinting { get; }
    event Action JumpAction;
}