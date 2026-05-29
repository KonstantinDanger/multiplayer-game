using Mirror;
using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCamera : NetworkBehaviour, IRotatablePlayerCamera
{
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private CameraConfig _config;
    [field: SerializeField] public Transform Transform { get; private set; }

    public Quaternion LookRotation => Quaternion.LookRotation(_cameraHolder.forward);
    public Vector3 Forward => _cameraHolder.forward;

    private float _verticalRotation;
    private float _horizontalRotation;

    public event Action<float> OnRotate;

    private bool _isLocked;
    private bool _initialized;

    public void Initialize(bool isLocalPlayer)
    {
        if (_initialized)
            return;

        if (!isLocalPlayer)
        {
            _cameraHolder.gameObject.SetActive(false);
            _cameraHolder.GetComponentInChildren<Camera>().tag = StaticData.Constants.EmptyTag;

            return;
        }

        HideCursor();

        _initialized = true;
    }

    /// <summary>
    /// Rotates body horizontally
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void Rotate(Vector3 direction, float speed)
    {
        if (_isLocked)
            return;

        _horizontalRotation = direction.x * speed * Time.deltaTime;

        _bodyTransform.Rotate(Vector3.up, _horizontalRotation);

        OnRotate?.Invoke(_horizontalRotation);
    }

    /// <summary>
    /// Vertical rotation (should be client-side)
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="speed"></param>
    public void RotateVertically(Vector3 direction, float speed)
    {
        if (_isLocked)
            return;

        float verticalDelta = direction.y * speed * Time.deltaTime;

        _verticalRotation -= verticalDelta;
        _verticalRotation = Mathf.Clamp(_verticalRotation, _config.MinRotationAngle, _config.MaxRotationAngle);

        _cameraHolder.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _isLocked = false;
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _isLocked = true;
    }
}
