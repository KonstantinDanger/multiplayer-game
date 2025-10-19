using UnityEngine;

public class ZoneView : MonoBehaviour
{
    [SerializeField] private Zone _zone;
    [SerializeField] private Transform _viewObject;

    private void OnValidate()
    {
        if (_zone.gameObject == _viewObject.gameObject)
        {
            Debug.LogError("Zone view object should not be same object with a Zone component.");
            _viewObject = null;
        }

    }

    private void OnEnable()
        => _zone.OnZoneShrink += HandleZoneShrink;

    private void OnDisable()
        => _zone.OnZoneShrink -= HandleZoneShrink;

    private void HandleZoneShrink(SphereCollider collider)
        => _viewObject.localScale = 2 * collider.radius * Vector3.one;
}

