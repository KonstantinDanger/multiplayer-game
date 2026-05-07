using System;
using System.Collections.Generic;

public interface IDamageable : IGauge
{
    event Action<Damage> ServerOnDamageTaken;
    event Action<Damage> ServerOnDemise;

    event Action<Damage> ClientOnDamageTaken;
    event Action<Damage> ClientOnDemise;

    public bool IsDead { get; }

    void Respawn();
    void TakeDamage(Damage damage);
    void Initialize(StatParameter baseHealth, IEnumerable<DamageHandler> damageHandlers);
}
