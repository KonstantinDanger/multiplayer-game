using AYellowpaper;
using Mirror;
using UnityEngine;

public class XpDrop : NetworkBehaviour
{
    [SerializeField] private InterfaceReference<IDamageable> _damageableRef;

    private IDamageable Damageable => _damageableRef.Value;

    private void OnEnable()
        => Damageable.OnDemise += HandleXpDrop;

    private void OnDisable()
        => Damageable.OnDemise -= HandleXpDrop;

    private void HandleXpDrop(Damage damage)
    {
        Level level = damage.Sender.GetComponent<Level>();
        EnemyConfig config = GetComponent<Enemy>().Config;

        float xpToDrop = config.XpToDrop;
        level.AddXp(xpToDrop);

        //CmdHandleXpDrop(damage);
    }

    [Command(requiresAuthority = false)]
    private void CmdHandleXpDrop(Damage damage)
        => ServerHandleXpDrop(damage);

    [Server]
    private void ServerHandleXpDrop(Damage damage)
    {
        Level level = damage.Sender.GetComponent<Level>();
        EnemyConfig config = GetComponent<Enemy>().Config;

        float xpToDrop = config.XpToDrop;
        level.AddXp(xpToDrop);
    }
}