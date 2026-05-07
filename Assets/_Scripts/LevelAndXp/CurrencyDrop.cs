using AYellowpaper;
using Mirror;
using UnityEngine;

public class CurrencyDrop : NetworkBehaviour
{
    [SerializeField] private InterfaceReference<IDamageable> _damageableRef;

    private IDamageable Damageable => _damageableRef.Value;

    private void OnEnable()
        => Damageable.ServerOnDemise += HandleDrop;

    private void OnDisable()
        => Damageable.ServerOnDemise -= HandleDrop;

    private void HandleDrop(Damage damage)
    {
        if (!damage.Sender)
            return;

        if (!damage.Sender.TryGetComponent(out Wallet wallet))
            return;

        EnemyConfig config = GetComponent<Enemy>().Config;

        int drop = config.CurrencyDrop;
        wallet.Add(CurrencyType.Match, drop);
    }
}