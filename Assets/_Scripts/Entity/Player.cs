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

    private IPlayerInputBrain Input => InputBrain as IPlayerInputBrain;
    private IRotatablePlayerCamera Camera => Rotatable as IRotatablePlayerCamera;
    private IPlayerDeathHandler PlayerDeathHandler { get; set; }

    private bool _isMenuActive = true;
    private LobbyUI _menu;

    private bool IsOffline =>
        !NetworkClient.active &&
        !NetworkServer.active;

    //public override void OnStartLocalPlayer()
    //{
    //    base.OnStartLocalPlayer();

    //    _thirdPersonModel.SetActive(false);
    //}

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (!isLocalPlayer)
            gameObject.layer = StaticData.Constants.EnemyLayer;
    }

    protected override void OnAwake()
    {
        //_menu = ServiceLocator.Container.Resolve<LobbyUI>();

        HandleMenuInvoked();

        var characterClass = _characterClass.GetNew();
        var abilities = characterClass.Abilities.ToList();

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

    [TargetRpc]
    public void SetCanAttack(bool canAttack)
        => Input.SetPlayerAttackInput(canAttack);

    [ClientRpc]
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
