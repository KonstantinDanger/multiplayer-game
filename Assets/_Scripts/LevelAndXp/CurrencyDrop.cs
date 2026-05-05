using AYellowpaper;
using Mirror;
using UnityEngine;

public class CurrencyDrop : NetworkBehaviour
{
    [SerializeField] private InterfaceReference<IDamageable> _damageableRef;

    private IDamageable Damageable => _damageableRef.Value;

    private void OnEnable()
        => Damageable.OnDemise += HandleDrop;

    private void OnDisable()
        => Damageable.OnDemise -= HandleDrop;

    private void HandleDrop(Damage damage)
    {
        if (!damage.Sender)
            return;

        if (!damage.Sender.TryGetComponent(out Wallet wallet))
            return;

        EnemyConfig config = GetComponent<Enemy>().Config;

        int drop = config.CurrencyDrop;
        wallet.Add(CurrencyType.Match, drop);

        //CmdHandleXpDrop(damage);
    }

    //[Command(requiresAuthority = false)]
    //private void CmdHandleXpDrop(Damage damage)
    //    => ServerHandleXpDrop(damage);

    //[Server]
    //private void ServerHandleXpDrop(Damage damage)
    //{
    //    Level level = damage.Sender.GetComponent<Level>();
    //    EnemyConfig config = GetComponent<Enemy>().Config;

    //    float xpToDrop = config.CurrencyDrop;
    //    level.AddXp(xpToDrop);
    //}
}