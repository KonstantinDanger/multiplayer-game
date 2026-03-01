using System;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public event Action<float> OnZoneShrink;

    [SerializeField] private float _minRadius;
    [SerializeField] private float _maxRadius;

    public float Radius { get; private set; }

    private void Start()
        => Radius = _maxRadius;

    public void Shrink(float normalizedValue)
    {
        //normalizedValue => [0, 1]
        //shrinkMultiplier => 1 - [0, 1];
        //apply to size by shrinkMultiplier

        float shrinkMultiplier = 1 - normalizedValue;

        float currentZoneRadius = Mathf.Lerp(_minRadius, _maxRadius, shrinkMultiplier);

        Radius = currentZoneRadius;

        OnZoneShrink.Invoke(Radius);
    }

    #region Debug
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, _minRadius);

        Gizmos.color = new Color(0, 255, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, _maxRadius);

        Gizmos.color = new Color(255, 0, 0, 0.5f);
        Gizmos.DrawSphere(transform.position, Radius);
    }
#endif
    #endregion
}
