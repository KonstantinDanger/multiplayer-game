public class Player : Entity
{
    private IPlayerInputBrain Input => InputBrain as IPlayerInputBrain;
    private IRotatablePlayerCamera Camera => Rotatable as IRotatablePlayerCamera;

    private bool _isMenuActive = true;

    private LobbyUI _menu;

    protected override void HandleOnEnable()
        => Input.OnMenuInvoked += HandleMenuInvoked;

    protected override void HandleOnDisable()
        => Input.OnMenuInvoked -= HandleMenuInvoked;

    protected override void OnAwake()
    {
        _menu = FindFirstObjectByType<LobbyUI>();

        HandleMenuInvoked();
    }

    //public void Initialize(LobbyUI menu)
    //    => _menu = menu;

    protected override void Update()
    {
        if (!isLocalPlayer)
            return;

        base.Update();
    }

    protected override void HandleJump()
    {
        if (!isLocalPlayer)
            return;

        base.HandleJump();
    }

    private void HandleMenuInvoked()
    {
        if (!isLocalPlayer)
            return;

        _isMenuActive = !_isMenuActive;

        Input.SetPlayerInput(!_isMenuActive);

        if (_isMenuActive)
        {
            Camera.ShowCursor();
        }
        else
        {
            Camera.HideCursor();
        }


        //Input.SetUiInput(_isMenuActive);

        _menu.gameObject.SetActive(_isMenuActive);
    }
}
