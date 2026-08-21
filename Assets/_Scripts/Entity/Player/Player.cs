using AYellowpaper;
using Mirror;
using Steamworks;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [SyncVar, HideInInspector] public ulong SteamID;
    [SyncVar, HideInInspector] public string SteamName;

    private IStateMachine _stateMachine;

    [SerializeField] private RayCastDamager _damager; //For testing
    [SerializeField] private ScriptableStatusEffect _statusEffectForTesting; //For testing
    [SerializeField] private Respawn _respawn;
    [SerializeField] private AbilityUser _abilities;
    [SerializeField] private ScriptableCharacterClass _baseCharacterClass;
    [SerializeField] private Level _level;
    [SerializeField] private Upgrader _upgrader;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private Interactor _interactor;
    [SerializeField] private InterfaceReference<IPlayerInputBrain> _inputRef;
    [SerializeField] private MatchStatusReceiver _matchStatusReceiver;
    [SerializeField] private WeaponUserComponent _weaponUserComponent;

    public ScriptableCharacterClass CharacterClass { get; private set; }
    public IPlayerInputBrain Input => _inputRef.Value;
    public IRotatablePlayerCamera Camera => Rotatable as IRotatablePlayerCamera;

    private bool _isUpgrading = false;
    private GameUI _gameUI;
    private LobbyView _menu;
    private UpgradeView _upgradeView;
    private LevelUpView _levelUpView;
    private PlayerHUD _playerHUD;
    private bool _isUICreated;
    private (bool toggle, bool active) _toggleHUDDirty;

    private bool IsOffline =>
        !NetworkClient.active &&
        !NetworkServer.active;

    private void Awake()
    {
        Input.Initialize();

        SetCharacterClass(_baseCharacterClass);

        _stateMachine = new PlayerStateMachine(this);

        CreateUI();

        if (HasAuthority())
            ServiceLocator.Container.RegisterSingle(_matchStatusReceiver);
    }

    protected override void HandleOnEnable()
    {
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

    public override void OnStartLocalPlayer()
    {
        CreateUI();

        CmdSetSteamData(
            SteamUser.GetSteamID().m_SteamID,
            SteamFriends.GetPersonaName());
    }

    public override void OnStartClient()
    {
        if (isClient)
            DontDestroyOnLoad(gameObject);

        base.OnStartClient();

        if (HasAuthority() && _stateMachine == null)
            _stateMachine = new PlayerStateMachine(this);

        _upgrader.Initialize();

        Camera.Initialize(HasAuthority());

        TeamId.Id = (int)netId;

        PlayerData data = ServiceLocator.Container.Resolve<PlayerData>();

        if (NetworkServer.active && NetworkClient.active)
            InitWallet(data.MetaCurrency);
        else
            CmdInitWallet(data.MetaCurrency);
    }

    [Command(requiresAuthority = false)]
    private void CmdInitWallet(int metaCurrencyAmount)
        => InitWallet(metaCurrencyAmount);

    private void InitWallet(int metaCurrencyAmount)
        => _wallet.Initialize(new() { [CurrencyType.Meta] = metaCurrencyAmount });

    protected override void OnEntityStartServer()
    {
        if (isLocalPlayer && isServer)
            gameObject.name += " (Server)";
    }

    protected override void Update()
    {
        if (!HasAuthority())
            return;

        #region For testing

#if UNITY_EDITOR
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

        var handler = GetComponent<StatusEffectHandler>();
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {

            handler.TryProc(_statusEffectForTesting, 10);


        }
        //UnityEngine.Debug.Log(handler);
        //UnityEngine.Debug.Log("Wallet " + _wallet.ToString());
        //UnityEngine.Debug.Log("Stats => " + Stats.ToString());

        //UnityEngine.Debug.Log(_level);
        //UnityEngine.Debug.Log("Current state: " + _stateMachine.CurrentState);
#endif
        #endregion

        Input.UpdateLogic();
        Camera.RotateVertically(Input.Rotation, RotationConfig.RotationSpeed);

        if (IsOffline)
        {
            UpdateStateMachine();
            HandleRotation(Input.Rotation);
        }
        else
        {
            CmdServerTick(Input.MovementVector, Input.Rotation, Input.IsSprinting);
        }

        _abilities.OnUpdate();

        base.Update();
    }

    [Command(requiresAuthority = false)]
    private void CmdServerTick(Vector2 movementVector, Vector3 rotation, bool isSprinting)
    {
        Input.SetInputState(movementVector, rotation, isSprinting);
        HandleRotation(rotation);
        UpdateStateMachine();
    }

    private void UpdateStateMachine()
        => _stateMachine.CurrentState.Update(Time.deltaTime);

    private void HandleRotation(Vector3 rotation)
        => Camera.Rotate(rotation, RotationConfig.RotationSpeed);

    private void HandleJump()
    {
        if (!HasAuthority())
            return;

        if (IsOffline)
        {
            Jump();
            return;
        }

        CmdHandleJump();
    }

    [Command(requiresAuthority = false)]
    private void CmdSetSteamData(ulong steamId, string steamName)
    {
        SteamID = steamId;
        SteamName = steamName;
    }

    private void Jump()
        => Movable.Jump(MovementConfig.JumpHeight, MovementConfig.Gravity);

    [Command(requiresAuthority = false)]
    private void CmdHandleJump() => Jump();

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

    [TargetRpc]
    public void SaveData()
    {
        var dataProvider = ServiceLocator.Container.Resolve<IDataProvider>();
        var data = dataProvider.Load<PlayerData>();

        data.MetaCurrency += _wallet.GetAmountOf(CurrencyType.Meta);
        data.Name = SteamName;

        dataProvider.Save(data);
    }

    public void SetCharacterClass(ScriptableCharacterClass @class)
    {
        if (!@class)
            throw new System.Exception("No class found!");

        CharacterClass = @class;

        _weaponUserComponent.Initialize(@class.GetWeaponUser());

        _abilities.Initialize(CharacterClass
            .GetNew()
            .Abilities
            .ToList(),
            @class.AbilityExecutionMatrix);
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

    private void HandleAttack()
    {
        if (!HasAuthority())
            return;

        CmdHandleAbility(0);
        //_damager.InflictDamage(Camera.Transform.position, Camera.Transform.forward);
    }

    [Command]
    private void CmdHandleAbility(int index)
        => _abilities.Use(index);

    #endregion

    #region UI
    private void CreateUI()
    {
        if (_isUICreated || !HasAuthority())
            return;

        GameFactory factory = ServiceLocator.Container.Resolve<GameFactory>();

        var data = ServiceLocator.Container.Resolve<StaticData>();
        var hudPrefab = data.PlayerHUDPrefab;
        var upgradeUIPrefab = data.UpgradeUIPrefab;
        var levelUpUIPrefab = data.LevelUpUIPrefab;
        var charSelectUIPrefab = data.CharacterSelectUI;

        //Game ui spawn
        _gameUI = factory
            .Create(data.UIPrefab, transform)
            .Initialize(HandleViewOpen, HandleAllViewsClose);

        _playerHUD = Instantiate(hudPrefab, _gameUI.transform);
        _levelUpView = Instantiate(levelUpUIPrefab, _gameUI.transform);
        _upgradeView = Instantiate(upgradeUIPrefab, _levelUpView.transform);
        CharacterSelectView charSelectInstance = Instantiate(charSelectUIPrefab, _gameUI.transform);
        _menu = Instantiate(data.LobbyUIPrefab, _gameUI.transform);

        _playerHUD.Initialize(_abilities, Damageable, _level, _respawn, _wallet);
        _upgradeView.Initialize(gameObject, _upgrader);
        _levelUpView.Initialize(_level, _wallet);
        charSelectInstance.Initialize(data.ClassList, this);
        _menu.Initialize(
            ServiceLocator.Container.Resolve<ILobby>(),
            _matchStatusReceiver);

        //UI addition to the Game ui
        _gameUI
            .Add(_menu)
            .Add(_playerHUD)
            .Add(_levelUpView)
            .Add(_upgradeView)
            .Add(charSelectInstance);

        ServiceLocator.Container.RegisterSingle(_gameUI);

        if (_toggleHUDDirty.toggle)
            ToggleMatchHUD(_toggleHUDDirty.active);

        _isUICreated = true;
    }

    [TargetRpc]
    public void UpdateMatchHUD(float matchTime, float normalizedProgress, bool isDeathmatchStarted)
    {
        MatchProgressHUD matchHUD = _playerHUD.Show<MatchProgressHUD>();
        matchHUD.UpdateProgress(matchTime, normalizedProgress, isDeathmatchStarted);
    }

    [TargetRpc]
    public void ShowMatchOutcome(MatchResult result, GameMatchData.Data matchSummaryData, float summaryDuration)
    {
        MatchResultScreenHUD hud = _playerHUD.Show<MatchResultScreenHUD>();
        hud.Initialize(result, matchSummaryData, summaryDuration);
        HandleViewOpen();
    }

    [TargetRpc]
    public void HideMatchOutcome()
    {
        _playerHUD.Hide<MatchResultScreenHUD>();
        HandleAllViewsClose();
    }

    [TargetRpc]
    public void TargetSetLobbyHUDActive(bool active)
        => ToggleLobbyHUD(active);

    public void ToggleLobbyHUD(bool active)
    {
        if (active)
        {
            _playerHUD.Show<LobbyHUD>();
            return;
        }

        _playerHUD.Hide<LobbyHUD>();
    }

    public void ToggleMatchHUD(bool active)
    {
        if (_playerHUD == null)
        {
            _toggleHUDDirty = (true, active);
            return;
        }

        _playerHUD.SetActive(active);

        _toggleHUDDirty = (false, active);
    }

    [TargetRpc]
    public void TargetRpcToggleMatchHUD(bool active)
        => ToggleMatchHUD(active);

    private void HandleViewOpen()
    {
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
        if (!HasAuthority() || Damageable.IsDead)
            return;

        if (_isUpgrading)
        {
            _gameUI.CloseView();
            _isUpgrading = false;
            return;
        }

        _gameUI.OpenViewOfType(_levelUpView.GetType());
        _isUpgrading = true;
    }

    private void HandleUICancel()
    {
        if (!HasAuthority() || Damageable.IsDead)
            return;

        if (_gameUI.HasStackedUIViews)
        {
            _gameUI.CloseView();
            return;
        }

        _gameUI.OpenViewOfType(_menu.GetType());
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

        ServiceLocator.Container.Dispose();
    }

    private void OnDestroy()
    {
        ServiceLocator.Container.Unregister<GameUI>();

        if (HasAuthority())
            ServiceLocator.Container.Unregister<MatchStatusReceiver>();

        Dispose();
    }
}
