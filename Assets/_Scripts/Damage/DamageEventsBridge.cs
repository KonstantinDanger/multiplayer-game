using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(DamageSystem))]
public class DamageEventsBridge : MonoBehaviour
{
    public UnityEvent OnDamageTaken;
    public UnityEvent OnDemise;

    [SerializeField] private DamageSystem _damageSystem;

    private void OnEnable()
    {
        _damageSystem.ClientOnDamageTaken += HandleDamageTaken;
        _damageSystem.ClientOnDemise += HandleDemise;
    }

    private void OnDisable()
    {
        _damageSystem.ClientOnDamageTaken -= HandleDamageTaken;
        _damageSystem.ClientOnDemise -= HandleDemise;
    }

    private void HandleDemise(Damage damage) => OnDemise?.Invoke();
    private void HandleDamageTaken(Damage damage) => OnDamageTaken?.Invoke();
}

