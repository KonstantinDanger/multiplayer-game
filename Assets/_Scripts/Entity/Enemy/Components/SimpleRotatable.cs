using System;
using UnityEngine;

public class SimpleRotatable : MonoBehaviour, IRotatable
{
    public Quaternion LookRotation { get; private set; }
    public Vector3 Forward => transform.forward;

    public event Action<float> OnRotate;

    public void Rotate(Vector3 direction, float speed)
    {
        if (direction == Vector3.zero)
            return;

        LookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, speed * Time.deltaTime);

        OnRotate?.Invoke(0f);
    }
}
