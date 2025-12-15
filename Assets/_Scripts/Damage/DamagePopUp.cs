using AYellowpaper;
using Mirror;
using UnityEngine;

public class DamagePopUp : NetworkBehaviour
{
    [SerializeField] private InterfaceReference<IDamageable> _damageable;
    [SerializeField] private Transform _textSpawnTransform;

    private DamageText _damageTextPrefab;
    private GameFactory _gameFactory;

    private IDamageable Damageable => _damageable.Value;


    private void OnEnable()
        => Damageable.OnDamageTaken += HandleDamageTaken;

    private void Start()
    {
        _damageTextPrefab = ServiceLocator.Container.Resolve<StaticData>().DamageTextPrefab;
        _gameFactory = ServiceLocator.Container.Resolve<GameFactory>();
    }

    private void OnDisable()
        => Damageable.OnDamageTaken -= HandleDamageTaken;

    private void HandleDamageTaken(Damage damage)
        => RpcHandleDamageTaken(damage);

    private void RpcHandleDamageTaken(Damage damage)
    {
        float takenDamage = damage.Amount;

        DamageText dText = _gameFactory.SpawnDamageText(_damageTextPrefab, _textSpawnTransform);

        dText.Text = takenDamage.ToString();
        dText.StartTextPopUp();
    }
}
