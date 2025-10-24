using System;
using System.Collections.Generic;

public interface IDamageable
{
    event Action<Damage> OnDamageTaken;
    event Action OnDemise;

    void Initialize(float baseHealth, IEnumerable<DamageHandler> damageHandlers);
    void RegenFully();
    void TakeDamage(Damage damage);
}
