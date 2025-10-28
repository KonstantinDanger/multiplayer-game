using System;
using System.Collections.Generic;

public interface IDamageable
{
    event Action<Damage> OnDamageTaken;
    event Action OnDemise;

    public bool IsDead { get; }

    void Initialize(float baseHealth, IEnumerable<DamageHandler> damageHandlers);
    void Respawn();
    void TakeDamage(Damage damage);
}
