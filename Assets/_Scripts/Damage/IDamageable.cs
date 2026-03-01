using System;
using System.Collections.Generic;

public interface IDamageable : IGauge
{
    event Action<Damage> OnDamageTaken;
    event Action<Damage> OnDemise;

    public bool IsDead { get; }

    void Respawn();
    void TakeDamage(Damage damage);
    void Initialize(StatParameter baseHealth, IEnumerable<DamageHandler> damageHandlers);
}
