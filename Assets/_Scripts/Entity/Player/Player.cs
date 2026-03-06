using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    private IStateMachine _stateMachine;

    [SerializeField] private RayCastDamager _damager; //For testing
    [SerializeField] private Respawn _respawn;
    [SerializeField] private GameObject _headObject;
    [SerializeField] private PlayerAbilityUser _abilities;
    [SerializeField] private ScriptableCharacterClass _characterClass;
    [SerializeField] private Level _level;
    [SerializeField] private Upgrader _upgrader;

    private IPlayerInputBrain Input => InputBrain as IPlayerInputBrain;
    private IRotatablePlayerCamera Camera => Rotatable as IRotatablePlayerCamera;
    public ScriptableCharacterClass CharacterClass => _characterClass;

    private AnimatorUpdater _animatorUpdater;
    private bool _isMenuActive = true;
    private bool _isUpgrading = false;
    private MainUI _playerUI;
    private LobbyUI _menu;
    private UpgradeUI _upgradeUI;
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
        _stateMachine = new PlayerStateMachine(this);
        //_menu = ServiceLocator.Container.Resolve<LobbyUI>();

        _upgrader.Initialize();
        _abilities.Initialize(_characterClass
            .GetNew()
            .Abilities
            .ToList());

        var animatorUpdData = ServiceLocator.Container.Resolve<StaticData>().AnimatorUpdaterConfig;

        _animatorUpdater = new(animatorUpdData);
    }

    protected override void HandleOnEnable()
    {
        Input.OnMenuInvoked += HandleMenuInvoked;
        Input.AttackAction += HandleAttack;
        Input.AbilityAction += HandleAbility;
        Input.OnUpgradeMenuInvoked += HandleUpgradeMenuInvoked;
    }

    protected override void HandleOnDisable()
    {
        Input.OnMenuInvoked -= HandleMenuInvoked;
        Input.AttackAction -= HandleAttack;
        Input.AbilityAction -= HandleAbility;
        Input.OnUpgradeMenuInvoked -= HandleUpgradeMenuInvoked;
    }

    protected override void OnStart()
        => Camera.Initialize(CanDoActions());

    protected override void Update()
    {
        if (!CanDoActions())
            return;

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            Damageable.TakeDamage(new() { Amount = 50 });
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            Stats.AddStatMultiplier(StatType.Health, 0.1f);

            UnityEngine.Debug.Log("pressed ");
            UnityEngine.Debug.Log(Stats.GetStatMultiplier(StatType.Health, 0));
        }

        _abilities.OnUpdate();
        _stateMachine.CurrentState.Update(Time.deltaTime);
        _animatorUpdater.Update(Time.deltaTime);

        //UnityEngine.Debug.Log("Current state: " + _stateMachine.CurrentState);

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
    {
        _playerHUD.gameObject.SetActive(active);
        _playerHUD.Initialize(_abilities, Damageable, _level, _upgrader);
    }

    [ClientRpc]
    public void Respawn()
        => _respawn.Execute(this.netId, DamageSystemConfig.RespawnTime);

    public void RefillHealth()
        => Damageable.Respawn();

    [TargetRpc]
    public void ResetLevel()
        => CmdResetLevel();

    [Command]
    private void CmdResetLevel()
        => _level.Initialize();

    [TargetRpc]
    public void CreateUI()
    {
        if (!CanDoActions())
            return;

        var data = ServiceLocator.Container.Resolve<StaticData>();
        var hudPrefab = data.PlayerHUDPrefab;
        var upgradeUIPrefab = data.UpgradeUIPrefab;

        _playerUI = Instantiate(data.UIPrefab, transform);
        _playerHUD = Instantiate(hudPrefab, _playerUI.transform);
        _upgradeUI = Instantiate(upgradeUIPrefab, _playerUI.transform);

        _upgradeUI.Initialize(_upgrader);
    }

    [TargetRpc]
    public void SetCanAttack(bool canAttack)
        => Input.SetPlayerAttackInput(canAttack);

    public void InitializeAnimatorUpdater(IEnumerable<Animator> animators)
        => _animatorUpdater.Initialize(gameObject, animators);

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

    private void HandleUpgradeMenuInvoked()
    {
        if (!CanDoActions())
            return;

        _isUpgrading = !_isUpgrading;

        if (!Damageable.IsDead)
            Input.SetPlayerInput(!_isUpgrading);

        if (_isUpgrading)
        {
            Camera.ShowCursor();
        }
        else
        {
            Camera.HideCursor();
        }

        _upgradeUI.gameObject.SetActive(_isUpgrading);
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
