public interface IPlayerInputBrain : IInputBrain
{
    event System.Action OnMenuInvoked;
    event System.Action OnAttackInvoked;

    void SetUiInput(bool active);
    void SetPlayerInput(bool active);
}
