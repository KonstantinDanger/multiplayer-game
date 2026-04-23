using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderTrigger : MonoBehaviour
{
    public Action<Collider> OnTriggerEntered;
    public Action<Collider> OnTriggerStayed;
    public Action<Collider> OnTriggerExited;

    private void OnValidate()
    {
        if (TryGetComponent(out Collider col))
        {
            if (!col.isTrigger)
                col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other) => OnTriggerEntered?.Invoke(other);

    private void OnTriggerStay(Collider other) => OnTriggerStayed?.Invoke(other);

    private void OnTriggerExit(Collider other) => OnTriggerExited?.Invoke(other);
}
