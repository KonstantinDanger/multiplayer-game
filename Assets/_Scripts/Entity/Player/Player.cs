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
        => CreateUI();

    public override void OnStartClient()
    {
        if (isClient)
            DontDestroyOnLoad(gameObject);

        base.OnStartClient();

        //if (!isLocalPlayer)
        //    gameObject.layer = StaticData.Constants.EnemyLayer;

        if (HasAuthority() && _stateMachine == null)
            _stateMachine = new PlayerStateMachine(this);

        _upgrader.Initialize();

        Camera.Initialize(HasAuthority());
    }

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
        _menu.Initialize(ServiceLocator.Container.Resolve<ILobby>());

        //UI addition to the Game ui
        _gameUI
            .Add(_menu)
            .Add(_playerHUD)
            .Add(_levelUpView)
            .Add(_upgradeView)
            .Add(charSelectInstance);

        ServiceLocator.Container.RegisterSingle(_gameUI, cached: true);

        if (_toggleHUDDirty.toggle)
            ToggleHUD(_toggleHUDDirty.active);

        _isUICreated = true;
    }

    [TargetRpc]
    public void ShowMatchOutcome(MatchResult result)
    {
        { }
        UnityEngine.Debug.Log("match result: " + result);
        //Invoke match result screen somehow
    }

    [TargetRpc]
    public void HideMatchOutcome()
    {
        //Somehow get match result screen and turn it off
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

        Destroy(_playerHUD);

        ServiceLocator.Container.Dispose();
    }

    private void OnDestroy()
        => Dispose();
}
