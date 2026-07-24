using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlindScreenEffectPlayer : MonoBehaviour
{
    [SerializeField] private float _maxExposure = 6f;

    [SerializeField]
    private AnimationCurve _exposureCurve = new(
        new Keyframe(0f, 1f),
        new Keyframe(0.1f, 0.9f),
        new Keyframe(0.35f, 0.5f),
        new Keyframe(1f, 0f));

    private ColorAdjustments colorAdjustments;
    private Coroutine blindRoutine;
    private float defaultExposure;

    public void Blind(float duration)
    {
        if (!FindFirstObjectByType<Volume>().profile.TryGet(out colorAdjustments))
        {
            Debug.LogError("Volume is missing a Color Adjustments override.");
            enabled = false;
            return;
        }

        if (blindRoutine != null)
            StopCoroutine(blindRoutine);

        defaultExposure = colorAdjustments.postExposure.value;

        blindRoutine = StartCoroutine(BlindRoutine(duration));
    }

    private IEnumerator BlindRoutine(float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / duration);
            float curve = _exposureCurve.Evaluate(t);

            colorAdjustments.postExposure.value =
                defaultExposure + curve * _maxExposure;

            yield return null;
        }

        colorAdjustments.postExposure.value = defaultExposure;
        blindRoutine = null;
    }
}
