using AYellowpaper;
using System;
using UnityEngine;

public class DamageVignette : MonoBehaviour
{
    [SerializeField] private InterfaceReference<IDamageable> _damageableRef;

    [Header("Vignette Settings")]
    [SerializeField, Range(0f, 1f)] private float _maxIntensity = 0.6f;
    [SerializeField, Range(0f, 1f)] private float _softness = 0.3f;
    [SerializeField] private float _fadeSpeed = 2f;

    private Material _vignetteMaterial;

    private float _currentIntensity = 0f;
    private float _targetIntensity = 0f;

    private IDamageable Damageable => _damageableRef.Value;

    void Start()
    {
        // Find the material from the renderer feature
        // You'll need to assign this manually or find it through the URP asset
        var feature = FindObjectOfType<DamageVignetteFeature>();
        if (feature != null)
        {
            // Material will be created by the feature
        }
    }
    private void OnEnable()
    => Damageable.OnDamageTaken += HandleDamageTaken;

    private void OnDisable()
        => Damageable.OnDamageTaken -= HandleDamageTaken;

    private void HandleDamageTaken(Damage damage)
    {
        float normalizedDamage = Mathf.Clamp01(damage.Amount / 100f);
        _targetIntensity = _maxIntensity * normalizedDamage;

        Invoke(nameof(ResetVignette), 0.5f);
    }

    void Update()
    {
        // Smoothly fade the vignette
        _currentIntensity = Mathf.Lerp(_currentIntensity, _targetIntensity, Time.deltaTime * _fadeSpeed);

        if (_vignetteMaterial != null)
        {
            _vignetteMaterial.SetFloat("_VignetteIntensity", _currentIntensity);
            _vignetteMaterial.SetFloat("_VignetteSoftness", _softness);
        }
    }

    private void ResetVignette() => _targetIntensity = 0f;

    public void SetVignetteMaterial(Material mat) => _vignetteMaterial = mat;
}