using UnityEngine;

public interface IPlayerInputBrain : IInputBrain
{
    event System.Action OnMenuInvoked;
    event System.Action OnUpgradeMenuInvoked;
    event System.Action OnInteraction;

    void Initialize();
    void SetUiInput(bool active);
    void SetPlayerInput(bool active);
    void SetPlayerAttackInput(bool active);
    void SetInputState(Vector2 movement, Vector2 rotation, bool sprinting);
}
