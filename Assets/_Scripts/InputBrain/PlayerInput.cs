using System;
using UnityEngine;

[Serializable]
public class PlayerInput : IPlayerInputBrain
{
    private readonly PlayerInputActions _actions;

    public Vector2 MovementVector { get; private set; }
    public Vector2 Rotation { get; private set; }

    public event Action JumpAction;
    public event Action OnMenuInvoked;

    public bool IsSprinting { get; private set; }

    public PlayerInput()
    {
        _actions = new();

        _actions.Player.Jump.performed += _ => JumpAction?.Invoke();
        _actions.UI.Menu.performed += _ => OnMenuInvoked?.Invoke();

        _actions.Player.Sprint.started += _ => IsSprinting = true;
        _actions.Player.Sprint.canceled += _ => IsSprinting = false;
    }

    public void Update()
    {
        MovementVector = _actions.Player.Move.ReadValue<Vector2>();
        Rotation = _actions.Player.Look.ReadValue<Vector2>();
    }

    public void OnEnable()
        => _actions.Enable();

    public void OnDisable()
        => _actions.Disable();

    public void SetUiInput(bool active)
        => (active ? (Action)_actions.UI.Enable : _actions.UI.Disable)();

    public void SetPlayerInput(bool active)
        => (active ? (Action)_actions.Player.Enable : _actions.Player.Disable)();
}
