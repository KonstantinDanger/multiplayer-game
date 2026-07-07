using System.Collections;
using UnityEngine;

public class SplashDamageView : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private AnimationCurve _alphaFadeCurve;

    private bool _started;
    private Material _material;

    public void StartView(float splashRadius)
    {
        if (_started)
            return;

        _material = _meshRenderer.material;

        float scale = splashRadius;

        _transform.localScale = new(scale, scale, scale);

        StartCoroutine(FadeRoutine(_fadeDuration));

        _started = true;
    }

    private IEnumerator FadeRoutine(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float normalizedAlpha = _alphaFadeCurve.Evaluate(elapsedTime / duration);

            SetAlpha(normalizedAlpha);

            yield return null;
        }

        SetAlpha(0f);
        Destroy(gameObject);
    }

    private void SetAlpha(float value)
    {
        Color color = _material.color;
        color.a = value;
        _material.color = color;
    }
}
