using UnityEngine;

public class XpDrop : MonoBehaviour
{
    [SerializeField] private IDamageable _damageable;
    [SerializeField] private EnemyConfig _config;

    private void OnEnable()
        => _damageable.OnDemise += HandleXpDrop;

    private void OnDisable()
        => _damageable.OnDemise -= HandleXpDrop;

    private void HandleXpDrop(Damage damage)
    {
        float xpToDrop = _config.XpToDrop;
        Level level = damage.Sender.GetComponent<Level>();
        level.AddXp(xpToDrop);
    }
}