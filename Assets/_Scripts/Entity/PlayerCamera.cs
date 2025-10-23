using Mirror;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerCamera : NetworkBehaviour, IRotatablePlayerCamera
{
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private CameraConfig _config;
    [field: SerializeField] public Transform Transform { get; private set; }

    private float _verticalRotation;
    private float _horizontalRotation;

    public void Initialize(bool isLocalPlayer)
    {
        if (!isLocalPlayer)
            _cameraHolder.gameObject.SetActive(false);

        HideCursor();
    }

    public void Rotate(Vector3 direction, float speed)
    {
        float verticalDelta = direction.y * speed * Time.deltaTime;
        _horizontalRotation = direction.x * speed * Time.deltaTime;

        _verticalRotation -= verticalDelta;
        _verticalRotation = Mathf.Clamp(_verticalRotation, _config.MinRotationAngle, _config.MaxRotationAngle);

        _cameraHolder.localEulerAngles = new(_verticalRotation, _cameraHolder.localEulerAngles.y, _cameraHolder.localEulerAngles.z);
        _bodyTransform.Rotate(Vector3.up, _horizontalRotation);
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
