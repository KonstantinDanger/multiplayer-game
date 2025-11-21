using System;
using UnityEngine;

public class EnemyRotation : MonoBehaviour, IRotatable
{
    public Quaternion LookRotation => throw new NotImplementedException();

    public Vector3 Forward => transform.forward;

    public event Action<float> OnRotate;

    public void Rotate(Vector3 direction, float speed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.03f * Time.deltaTime);

        //float verticalDelta = direction.y * speed * Time.deltaTime;
        //_horizontalRotation = direction.x * speed * Time.deltaTime;

        //_verticalRotation -= verticalDelta;
        //_verticalRotation = Mathf.Clamp(_verticalRotation, _config.MinRotationAngle, _config.MaxRotationAngle);

        //_cameraHolder.localEulerAngles = new(_verticalRotation, _cameraHolder.localEulerAngles.y, _cameraHolder.localEulerAngles.z);

        //_bodyTransform.Rotate(Vector3.up, _horizontalRotation);

        //OnRotate?.Invoke(_horizontalRotation);
    }
}
