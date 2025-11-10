using AYellowpaper;
using Mirror;
using UnityEngine;

public class DamagePopUp : NetworkBehaviour
{
    [SerializeField] private InterfaceReference<IDamageable> _damageable;
    [SerializeField] private Transform _textSpawnPosition;

    private DamageText _damageTextPrefab;
    private GameFactory _gameFactory;

    private IDamageable Damageable => _damageable.Value;


    private void OnEnable() => Damageable.OnDamageTaken += HandleDamageTaken;

    private void Start()
    {
        _damageTextPrefab = ServiceLocator.Container.Resolve<StaticData>().DamageTextPrefab;
        _gameFactory = ServiceLocator.Container.Resolve<GameFactory>();
    }

    private void OnDisable() => Damageable.OnDamageTaken -= HandleDamageTaken;

    [Command(requiresAuthority = false)]
    private void HandleDamageTaken(Damage Damage)
    {
        //float takenDamage = Damage.Amount;

        DamageText dText = _gameFactory.SpawnDamageText(_damageTextPrefab, _textSpawnPosition.position);

        dText.Text = "Damage taken";
        dText.StartTextPopUp();
    }
}
