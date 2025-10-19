public interface IRotatablePlayerCamera : IRotatable
{
    void HideCursor();
    void ShowCursor();
    void Initialize(bool isLocalPlayer);
}
