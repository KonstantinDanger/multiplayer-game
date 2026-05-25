using System;
using UnityEngine;

public interface IInputBrain
{
    void UpdateLogic();
    void Enable();
    void Disable();

    Vector2 MovementVector { get; }
    Vector2 Rotation { get; }
    bool IsSprinting { get; }
    event Action JumpAction;
    event Action AttackAction;
    event Action<int> AbilityAction;

}