using Mirror;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private RayCastDamager _damager; //For testing

    private IPlayerInputBrain Input => InputBrain as IPlayerInputBrain;
    private IRotatablePlayerCamera Camera => Rotatable as IRotatablePlayerCamera;

    private bool _isMenuActive = true;
    private LobbyUI _menu;

    private bool IsOffline =>
        !NetworkClient.active &&
        !NetworkServer.active;

    protected override void HandleOnEnable()
    {
        Input.OnMenuInvoked += HandleMenuInvoked;
        Input.OnAttackInvoked += HandleAttack;
    }

    protected override void HandleOnDisable()
        => Input.OnMenuInvoked -= HandleMenuInvoked;

    protected override void OnAwake()
    {
        _menu = ServiceLocator.Container.Resolve<LobbyUI>();

        HandleMenuInvoked();
    }

    protected override void OnStart()
        => Camera.Initialize(CanDoActions());

    protected override void Update()
    {

        //UnityEngine.Debug.Log("Is player offline: " + IsOffline);
        //UnityEngine.Debug.Log("Is local player: " + isLocalPlayer);

        if (!CanDoActions())
            return;

        base.Update();
    }

    protected override void HandleJump()
    {
        if (!CanDoActions())
            return;

        base.HandleJump();
    }

    private void HandleAttack()
    {
        if (!CanDoActions())
            return;

        _damager.InflictDamage(Camera.Transform.position, Camera.Transform.forward);
    }

    private void HandleMenuInvoked()
    {
        if (!CanDoActions())
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

    private bool CanDoActions()
        => isLocalPlayer || IsOffline;
}
