using System.Collections;
using UnityEngine;

public class DamageTakenEffect : MonoBehaviour
{
    public enum FlickerMode
    {
        ColorTint,      // Multiply/tint the material color 
        AlphaFade,      // Toggle alpha/visibility
        MaterialSwap    // Swap to a separate "hit" material briefly
    }

    [Header("General Settings")]
    [Tooltip("How the flicker should visually behave.")]
    public FlickerMode mode = FlickerMode.ColorTint;

    [Tooltip("Total duration of the single flicker, in seconds.")]
    [Min(0.01f)]
    public float flickerDuration = 0.15f;

    [Tooltip("How many on/off blinks occur within the flicker duration. 1 = simple flash.")]
    [Min(1)]
    public int blinkCount = 2;

    [Tooltip("If true, uses unscaled time (keeps flickering even if Time.timeScale = 0).")]
    public bool useUnscaledTime = false;

    [Header("Color Tint Settings (if mode = ColorTint)")]
    public Color flickerColor = Color.red;

    [Header("Alpha Fade Settings (if mode = AlphaFade)")]
    [Range(0f, 1f)]
    public float fadeAlpha = 0.2f;

    [Header("Material Swap Settings (if mode = MaterialSwap)")]
    public Material hitMaterial;

    [Header("Renderers")]
    [Tooltip("Leave empty to auto-grab all Renderers in children.")]
    public Renderer[] targetRenderers;

    private Color[] _originalColors;
    private Material[] _originalMaterials;
    private Coroutine _flickerRoutine;

#if UNITY_EDITOR
    private void OnValidate()
        => targetRenderers = GetComponentsInChildren<Renderer>();
#endif
    private void Awake()
    {
        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = GetComponentsInChildren<Renderer>();

        CacheOriginals();
    }

    public void TriggerFlicker()
    {
        if (_flickerRoutine != null)
            StopCoroutine(_flickerRoutine);

        ResetVisuals();
        _flickerRoutine = StartCoroutine(FlickerRoutine());
    }

    private void CacheOriginals()
    {
        _originalColors = new Color[targetRenderers.Length];
        _originalMaterials = new Material[targetRenderers.Length];

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            var r = targetRenderers[i];
            if (r == null) continue;

            _originalMaterials[i] = r.sharedMaterial;

            if (r.material.HasProperty("_Color"))
                _originalColors[i] = r.material.color;
            else
                _originalColors[i] = Color.white;
        }
    }

    private IEnumerator FlickerRoutine()
    {
        float segment = flickerDuration / (blinkCount * 2f);
        bool flickerOn = true;

        for (int i = 0; i < blinkCount * 2; i++)
        {
            SetFlickerState(flickerOn);
            flickerOn = !flickerOn;

            if (useUnscaledTime)
                yield return new WaitForSecondsRealtime(segment);
            else
                yield return new WaitForSeconds(segment);
        }

        ResetVisuals();
        _flickerRoutine = null;
    }

    private void SetFlickerState(bool on)
    {
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            var r = targetRenderers[i];
            if (r == null) continue;

            switch (mode)
            {
                case FlickerMode.ColorTint:
                    r.material.color = on ? flickerColor : _originalColors[i];
                    break;

                case FlickerMode.AlphaFade:
                    Color c = _originalColors[i];
                    c.a = on ? fadeAlpha : _originalColors[i].a;
                    r.material.color = c;
                    break;

                case FlickerMode.MaterialSwap:
                    if (hitMaterial != null)
                        r.material = on ? hitMaterial : _originalMaterials[i];
                    break;
            }
        }
    }

    private void ResetVisuals()
    {
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            var r = targetRenderers[i];
            if (r == null) continue;

            if (mode == FlickerMode.MaterialSwap)
                r.material = _originalMaterials[i];
            else
                r.material.color = _originalColors[i];
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (targetRenderers[i] != null && targetRenderers[i].material != null)
                Destroy(targetRenderers[i].material);
        }
    }
}
