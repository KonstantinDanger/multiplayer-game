using AYellowpaper;
using Mirror;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    private IStateMachine _stateMachine;

    [SerializeField] private RayCastDamager _damager; //For testing
    [SerializeField] private Respawn _respawn;
    [SerializeField] private AbilityUser _abilities;
    [SerializeField] private ScriptableCharacterClass _baseCharacterClass;
    [SerializeField] private Level _level;
    [SerializeField] private Upgrader _upgrader;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private Interactor _interactor;

    [SerializeField] private InterfaceReference<IPlayerInputBrain> _inputRef;

    public ScriptableCharacterClass CharacterClass { get; private set; }
    public IPlayerInputBrain Input => _inputRef.Value;
    public IRotatablePlayerCamera Camera => Rotatable as IRotatablePlayerCamera;

    private bool _isMenuActive = false;
    private bool _isUpgrading = false;
    private GameUI _playerUI;
    private LobbyView _menu;
    private UpgradeView _upgradeUI;
    private LevelUpView _levelUpUI;
    private PlayerHUD _playerHUD;
    private bool _isUICreated;
    private (bool toggle, bool active) _toggleHUDDirty;

    private bool IsOffline =>
        !NetworkClient.active &&
        !NetworkServer.active;

    private void Awake()
    {
        if (HasAuthority())
            _stateMachine = new PlayerStateMachine(this);

        Input.Initialize();

        SetCharacterClass(_baseCharacterClass);

        if (HasAuthority())
            CreateUI();
    }

    protected override void HandleOnEnable()
    {
        if (!HasAuthority())
            return;

        Input.Enable();
        Input.JumpAction += HandleJump;
        Input.OnMenuInvoked += HandleUICancel;
        Input.AttackAction += HandleAttack;
        Input.AbilityAction += HandleAbility;
        Input.OnUpgradeMenuInvoked += HandleUpgradeMenuInvoked;
        Input.OnInteraction += HandleInteraction;
    }

    protected override void HandleOnDisable()
    {
        if (!HasAuthority())
            return;

        Input.Disable();
        Input.JumpAction -= HandleJump;
        Input.OnMenuInvoked -= HandleUICancel;
        Input.AttackAction -= HandleAttack;
        Input.AbilityAction -= HandleAbility;
        Input.OnUpgradeMenuInvoked -= HandleUpgradeMenuInvoked;
        Input.OnInteraction -= HandleInteraction;
    }

    private void Start()
        => Camera.Initialize(HasAuthority());

    public override void OnStartClient()
    {
        if (isClient)
            DontDestroyOnLoad(gameObject);

        base.OnStartClient();

        if (!isLocalPlayer)
            gameObject.layer = StaticData.Constants.EnemyLayer;
    }

    protected override void OnEntityStartServer()
    {
        if (isLocalPlayer && isServer)
            gameObject.name += " (Server)";

        if (_stateMachine == null)
            _stateMachine = new PlayerStateMachine(this);

        _upgrader.Initialize();
    }

    protected override void Update()
    {
        if (isServer || IsOffline)
            _stateMachine.CurrentState.Update(Time.deltaTime);

        if (!HasAuthority())
            return;

        #region For testing
        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            Damageable.TakeDamage(new() { Amount = 50 });
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            Stats.AddStatMultiplier(StatType.Health, 0.1f);

            UnityEngine.Debug.Log("pressed ");
            UnityEngine.Debug.Log(Stats.GetStatMultiplier(StatType.Health));
        }

        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            UnityEngine.Debug.Log("Current player max health: " + Damageable.MaxGaugeValue);
        }
        //UnityEngine.Debug.Log("Wallet " + _wallet.ToString());
        //UnityEngine.Debug.Log("Stats => " + Stats.ToString());

        //UnityEngine.Debug.Log(_level);
        //UnityEngine.Debug.Log("Current state: " + _stateMachine.CurrentState);
        #endregion

        Input.UpdateLogic();

        if (IsOffline)
            HandleLocomotion(Input.MovementVector, Input.Rotation, Input.IsSprinting);
        else
            CmdHandleLocomotion(Input.MovementVector, Input.Rotation, Input.IsSprinting);

        _abilities.OnUpdate();

        base.Update();
    }

    [Command(requiresAuthority = false)]
    private void CmdHandleLocomotion(Vector2 movement, Vector2 rotation, bool isSprinting)
        => HandleLocomotion(movement, rotation, isSprinting);

    private void HandleLocomotion(Vector2 movement, Vector2 rotation, bool isSprinting)
        => Rotatable.Rotate(rotation, RotationConfig.RotationSpeed);

    private void HandleJump()
    {
        if (!HasAuthority())
            return;

        Movable.Jump(MovementConfig.JumpHeight, MovementConfig.Gravity);
    }

    protected override void OnDemise(Damage damage)
    {
        Spectate(true);

        CmdOnDemise(this.netId);
    }

    [Command(requiresAuthority = false)]
    private void CmdOnDemise(uint netId)
        => Events.InvokePlayerDemise(netId);

    [ClientRpc]
    public void Respawn()
        => _respawn.Execute(this.netId, DamageSystemConfig.RespawnTime);

    public void RefillHealth()
    {
        if (!HasAuthority())
            return;

        Damageable.Respawn();
    }

    public void ResetLevel()
        => _level.Initialize();

    [TargetRpc]
    public void SetCanAttack(bool canAttack)
        => Input.SetPlayerAttackInput(canAttack);

    [TargetRpc]
    public void Spectate(bool active)
        => Input.SetPlayerInput(!active);

    public void SetCharacterClass(ScriptableCharacterClass @class)
    {
        if (!@class)
            throw new System.Exception("No class found!");

        CharacterClass = @class;

        _abilities.Initialize(CharacterClass
            .GetNew()
            .Abilities
            .ToList());
    }

    private void HandleInteraction()
    {
        if (!HasAuthority())
            return;

        if (IsOffline)
        {
            _interactor.Interact();
            return;
        }

        CmdHandleInteraction();
    }

    [Command(requiresAuthority = false)]
    private void CmdHandleInteraction()
        => _interactor.Interact();

    #region Ability
    private void HandleAbility(int index)
    {
        if (!HasAuthority() || index == 0)
            return;

        CmdHandleAbility(index);
    }

    [Command]
    private void CmdHandleAbility(int index)
        => _abilities.Use(index);

    private void HandleAttack()
    {
        if (!HasAuthority())
            return;

        CmdHandleAbility(0);
        //_damager.InflictDamage(Camera.Transform.position, Camera.Transform.forward);
    }
    #endregion

    #region UI
    private void CreateUI()
    {
        if (_isUICreated)
            return;

        var data = ServiceLocator.Container.Resolve<StaticData>();
        var hudPrefab = data.PlayerHUDPrefab;
        var upgradeUIPrefab = data.UpgradeUIPrefab;
        var levelUpUIPrefab = data.LevelUpUIPrefab;
        var charSelectUIPrefab = data.CharacterSelectUI;

        //Game ui spawn
        _playerUI = Instantiate(data.UIPrefab, transform);
        _playerUI.Initialize(HandleViewOpen, HandleAllViewsClose);

        _playerHUD = Instantiate(hudPrefab, _playerUI.transform);
        _levelUpUI = Instantiate(levelUpUIPrefab, _playerUI.transform);
        _upgradeUI = Instantiate(upgradeUIPrefab, _levelUpUI.transform);
        CharacterSelectView charSelectInstance = Instantiate(charSelectUIPrefab, _playerUI.transform);
        _menu = Instantiate(data.LobbyUIPrefab, _playerUI.transform);

        _playerHUD.Initialize(_abilities, Damageable, _level, _upgrader, _wallet);
        _upgradeUI.Initialize(gameObject, _upgrader);
        _levelUpUI.Initialize(_level, _wallet);
        charSelectInstance.Initialize(data.ClassList, this);
        _menu.Initialize(ServiceLocator.Container.Resolve<ILobby>());

        //UI addition to the Game ui
        _playerUI.Add(_playerHUD);
        _playerUI.Add(_levelUpUI);
        _playerUI.Add(_upgradeUI);
        _playerUI.Add(charSelectInstance);

        ServiceLocator.Container.RegisterSingle(_playerUI, cached: true);

        if (_toggleHUDDirty.toggle)
            ToggleHUD(_toggleHUDDirty.active);

        _isUICreated = true;
    }

    public void ToggleHUD(bool active)
    {
        if (_playerHUD == null)
        {
            _toggleHUDDirty = (true, active);
            return;
        }

        _playerHUD.gameObject.SetActive(active);

        _toggleHUDDirty = (false, active);
    }

    [TargetRpc]
    public void TargetRpcToggleHUD(bool active)
        => ToggleHUD(active);

    private void HandleViewOpen()
    {
        if (!HasAuthority())
            return;

        Camera.ShowCursor();
        Input.SetPlayerInput(false);
    }

    private void HandleAllViewsClose()
    {
        if (!HasAuthority())
            return;

        Camera.HideCursor();
        Input.SetPlayerInput(true);
    }

    private void HandleUpgradeMenuInvoked()
    {
        if (!HasAuthority())
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

        _levelUpUI.gameObject.SetActive(_isUpgrading);
    }

    private void HandleUICancel()
    {
        if (!HasAuthority())
            return;

        if (Damageable.IsDead)
            return;

        if (_playerUI.HasStackedUIViews)
        {
            _playerUI.CloseView();
            return;
        }

        _isMenuActive = !_isMenuActive;

        if (_isMenuActive)
        {
            HandleViewOpen();
            _menu.gameObject.SetActive(_isMenuActive);
            return;
        }

        HandleAllViewsClose();

        //Input.SetUiInput(_isMenuActive);

        _menu.gameObject.SetActive(_isMenuActive);
    }

    #endregion


    /// <summary>
    /// Checks if the player is local OR is offline
    /// </summary>
    /// <returns></returns>
    public bool HasAuthority()
        => isLocalPlayer || IsOffline;

    public override void Dispose()
    {
        base.Dispose();

        Destroy(_playerHUD);

        ServiceLocator.Container.Dispose();
    }

    private void OnDestroy()
        => Dispose();
}
