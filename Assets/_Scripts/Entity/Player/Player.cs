using Mirror;
using System.Linq;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private RayCastDamager _damager; //For testing
    [SerializeField] private Respawn _respawn;
    [SerializeField] private GameObject _headObject;
    [SerializeField] private Abilities _abilities;
    [SerializeField] private ScriptableCharacterClass _characterClass;
    [SerializeField] private Level _level;

    private IPlayerInputBrain Input => InputBrain as IPlayerInputBrain;
    private IRotatablePlayerCamera Camera => Rotatable as IRotatablePlayerCamera;
    public ScriptableCharacterClass CharacterClass => _characterClass;

    private bool _isMenuActive = true;
    private MainUI _playerUI;
    private LobbyUI _menu;
    private PlayerHUD _playerHUD;

    private bool IsOffline =>
        !NetworkClient.active &&
        !NetworkServer.active;

    protected override IInputBrain SetInputBrain()
        => new PlayerInput();

    public override void OnStartClient()
    {
        if (isClient)
            DontDestroyOnLoad(gameObject);

        base.OnStartClient();

        if (!isLocalPlayer)
            gameObject.layer = StaticData.Constants.EnemyLayer;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (isLocalPlayer && isServer)
            gameObject.name += " (Server)";
    }

    protected override void OnAwake()
    {
        //_menu = ServiceLocator.Container.Resolve<LobbyUI>();

        _isMenuActive = true;
        HandleMenuInvoked();

        _abilities.Initialize(_characterClass
            .GetNew()
            .Abilities
            .ToList());
    }

    protected override void HandleOnEnable()
    {
        Input.OnMenuInvoked += HandleMenuInvoked;
        Input.AttackAction += HandleAttack;
        Input.AbilityAction += HandleAbility;
    }

    protected override void HandleOnDisable()
    {
        Input.OnMenuInvoked -= HandleMenuInvoked;
        Input.AttackAction -= HandleAttack;
        Input.AbilityAction -= HandleAbility;
    }

    protected override void OnStart()
        => Camera.Initialize(CanDoActions());

    protected override void Update()
    {
        if (!CanDoActions())
            return;

        base.Update();
    }

    public override void Dispose()
    {
        base.Dispose();

        Destroy(_playerHUD);
    }

    protected override void HandleJump()
    {
        if (!CanDoActions())
            return;

        base.HandleJump();
    }

    protected override void OnDemise(Damage damage)
    {
        Spectate(true);

        CmdOnDemise(this.netId);
    }

    [Command(requiresAuthority = false)]
    private void CmdOnDemise(uint netId)
        => Events.InvokePlayerDemise(netId);

    [TargetRpc]
    public void ToggleHUD(bool active)
        => _playerHUD.gameObject.SetActive(active);

    [ClientRpc]
    public void Respawn()
        => _respawn.Execute(this.netId, DamageSystemConfig.RespawnTime);

    [TargetRpc]
    public void ResetLevel()
        => _level.Initialize();

    [TargetRpc]
    public void CreateUI()
    {
        if (!CanDoActions())
            return;

        var data = ServiceLocator.Container.Resolve<StaticData>();
        var hudPrefab = data.PlayerHUDPrefab;

        _playerUI = Instantiate(data.UIPrefab, transform);
        _playerHUD = Instantiate(hudPrefab, _playerUI.transform);
        _playerHUD.Initialize(_abilities, Damageable, _level);
    }

    [TargetRpc]
    public void SetCanAttack(bool canAttack)
        => Input.SetPlayerAttackInput(canAttack);

    public void Spectate(bool active)
        => Input.SetPlayerInput(!active);

    private void HandleAbility(int index)
    {
        if (!CanDoActions() || index == 0)
            return;

        _abilities.Use(index);
    }

    private void HandleAttack()
    {
        if (!CanDoActions())
            return;

        _abilities.Use();
        //_damager.InflictDamage(Camera.Transform.position, Camera.Transform.forward);
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

    private void LateUpdate()
    {
        if (!CanDoActions())
            return;

        _headObject.transform.position = Camera.Transform.position;
    }

    private bool CanDoActions()
        => isLocalPlayer || IsOffline;
}
