using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerInput : IPlayerInputBrain
{
    private readonly PlayerInputActions _actions;

    public Vector2 MovementVector { get; private set; }
    public Vector2 Rotation { get; private set; }

    public event Action JumpAction;
    public event Action OnMenuInvoked;
    public event Action AttackAction;
    public event Action<int> AbilityAction;
    public event Action OnUpgradeMenuInvoked;
    public event Action OnInteraction;

    public bool IsSprinting { get; private set; }

    public PlayerInput()
    {
        _actions = new();

        _actions.Player.Attack.performed += _ => AttackAction?.Invoke();
        _actions.Player.Ability.performed += HandleAbilityPerformed;

        _actions.Player.Jump.performed += _ => JumpAction?.Invoke();
        _actions.UI.Menu.performed += _ => OnMenuInvoked?.Invoke();
        _actions.UI.Upgrade.performed += _ => OnUpgradeMenuInvoked?.Invoke();

        _actions.Player.Sprint.started += _ => IsSprinting = true;
        _actions.Player.Sprint.canceled += _ => IsSprinting = false;

        _actions.Player.Interact.performed += _ => OnInteraction?.Invoke();
    }

    public void Update()
    {
        MovementVector = _actions.Player.Move.ReadValue<Vector2>();
        Rotation = _actions.Player.Look.ReadValue<Vector2>();
    }

    private void HandleAbilityPerformed(InputAction.CallbackContext context)
    {
        string keyName = context.control.name;

        int.TryParse(keyName, out int abilityIndex);

        AbilityAction?.Invoke(abilityIndex);
    }

    public void Enable()
        => _actions.Enable();

    public void Disable()
        => _actions.Disable();

    public void SetUiInput(bool active)
    {
        if (!_actions.Player.enabled)

            (active ? (Action)_actions.UI.Enable : _actions.UI.Disable)();
    }

    public void SetPlayerAttackInput(bool active)
    {
        (active ? (Action)_actions.Player.Attack.Enable : _actions.Player.Attack.Disable)();
        (active ? (Action)_actions.Player.Ability.Enable : _actions.Player.Ability.Disable)();
    }

    public void SetPlayerInput(bool active)
        => (active ? (Action)_actions.Player.Enable : _actions.Player.Disable)();
}
