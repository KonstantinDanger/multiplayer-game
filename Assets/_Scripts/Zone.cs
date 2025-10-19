using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Zone : MonoBehaviour
{
    public event Action<SphereCollider> OnZoneShrink;

    [SerializeField] private SphereCollider _collider;
    [SerializeField] private float _minRadius;
    [SerializeField] private float _maxRadius;

    public float CurrentRadius => _collider.radius;

    private void OnValidate()
    {
        if (_collider == null)
            TryGetComponent(out _collider);

        if (_collider && !_collider.isTrigger)
            _collider.isTrigger = true;
    }

    private void Start()
        => _collider.radius = _maxRadius;

    public void Shrink(float normalizedValue)
    {
        //normalizedValue => [0, 1]
        //shrinkMultiplier => 1 - [0, 1];
        //apply to size by shrinkMultiplier

        float shrinkMultiplier = 1 - normalizedValue;

        float currentZoneRadius = Mathf.Lerp(_minRadius, _maxRadius, shrinkMultiplier);

        _collider.radius = currentZoneRadius;

        OnZoneShrink.Invoke(_collider);
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
        Gizmos.DrawSphere(transform.position, _collider.radius);
    }
#endif
    #endregion
}
