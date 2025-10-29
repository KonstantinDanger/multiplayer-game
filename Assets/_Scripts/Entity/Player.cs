using Mirror;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private RayCastDamager _damager; //For testing
    [SerializeField] private Respawn _respawn;
    [SerializeField] private GameObject _thirdPersonModel;

    private IPlayerInputBrain Input => InputBrain as IPlayerInputBrain;
    private IRotatablePlayerCamera Camera => Rotatable as IRotatablePlayerCamera;
    private IPlayerDeathHandler PlayerDeathHandler { get; set; }

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

    //public override void OnStartLocalPlayer()
    //{
    //    base.OnStartLocalPlayer();

    //    _thirdPersonModel.SetActive(false);
    //}

    protected override void OnAwake()
    {
        //_menu = ServiceLocator.Container.Resolve<LobbyUI>();
        { }

        HandleMenuInvoked();
    }

    protected override void OnStart()
        => Camera.Initialize(CanDoActions());

    public void Initialize(Match match)
        => PlayerDeathHandler = new PlayerDeathHandler(match);

    protected override void Update()
    {
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

    protected override void OnDemise()
    {
        Spectate(true);

        void RespawnAction()
            => _respawn.Execute(this, DamageSystemConfig.RespawnTime);

        PlayerDeathHandler.HandleDeath(RespawnAction);
    }

    public void Respawn()
        => Damageable.Respawn();

    [ClientRpc]
    public void Spectate(bool active)
        => Input.SetPlayerInput(!active);

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

        if (!Damageable.IsDead)
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

        //_menu.gameObject.SetActive(_isMenuActive);
    }

    private bool CanDoActions()
        => isLocalPlayer || IsOffline;
}
