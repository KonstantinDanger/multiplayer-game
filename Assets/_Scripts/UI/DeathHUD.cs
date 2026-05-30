using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathHUD : HUD
{
    [SerializeField] private Image _respawnProgressThrobber;

    private IDamageable _damageable;
    private Respawn _respawn;

    private bool _initialized;
    private bool _isRunning;

    public void Initialize(IDamageable damageable, Respawn respawn)
    {
        if (_initialized)
            return;

        _damageable = damageable;
        _respawn = respawn;

        _damageable.ClientOnDemise += HandleDemise;

        _initialized = true;
    }

    private void OnDestroy()
        => _damageable.ClientOnDemise -= HandleDemise;

    private void HandleDemise(Damage damage)
    {
        if (_isRunning)
            StopCoroutine(RespawnRoutine());

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        gameObject.SetActive(true);
        _isRunning = true;

        while (_respawn.Progress < 1.0f)
        {
            _respawnProgressThrobber.fillAmount = 1 - _respawn.Progress;
            yield return null;
        }

        gameObject.SetActive(false);
        _isRunning = false;
    }
}
