using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] private Transform _boneToRotate;
    [SerializeField] private Transform _camera;

    private void LateUpdate()
    {
        Vector3 eulerAngles = _boneToRotate.localEulerAngles;
        eulerAngles.y = -_camera.eulerAngles.x;
        _boneToRotate.localEulerAngles = eulerAngles;
    }
}
