public interface IPlayerInputBrain : IInputBrain
{
    event System.Action OnMenuInvoked;

    void SetUiInput(bool active);
    void SetPlayerInput(bool active);
}
