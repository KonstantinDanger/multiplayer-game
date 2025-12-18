using UnityEngine;

public class PlayerIK : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _spineTransform;

    private void LateUpdate()
    {
        float verticalAngle = _cameraTransform.eulerAngles.x;

        _spineTransform.localEulerAngles = new Vector3(0, 0, verticalAngle);
    }
}
