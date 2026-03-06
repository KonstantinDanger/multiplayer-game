public interface IPlayerInputBrain : IInputBrain
{
    event System.Action OnMenuInvoked;
    event System.Action OnUpgradeMenuInvoked;

    void SetUiInput(bool active);
    void SetPlayerInput(bool active);
    void SetPlayerAttackInput(bool active);
}
