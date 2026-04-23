using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ElevatorPlatform : MonoBehaviour
{
    [field: SerializeField] public Transform Pivot { get; private set; }
    [field: SerializeField] public Transform ObjectsHolder { get; private set; }
    [field: SerializeField] public ColliderTrigger PressurePlate { get; private set; }

    private void OnValidate()
    {
        if (TryGetComponent(out Collider col))
        {
            if (col.isTrigger)
            {
                UnityEngine.Debug.LogWarning("Collider of an elevator platform should not be trigger");
                col.isTrigger = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;

        if (IsPlayer(go, out Player player))
        {
            player.Movement.ResetVerticalVelocity();
            player.Movement.ApplyGravity = false;
        }

        go.transform.SetParent(ObjectsHolder, false);
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject go = collision.gameObject;

        if (IsPlayer(go, out Player player))
            player.Movement.ApplyGravity = true;

        go.transform.SetParent(null, true);
    }

    private bool IsPlayer(GameObject gameObject, out Player player)
    {
        return gameObject.TryGetComponent(out player);
    }

    private void SetParentTo(GameObject gameObject, Transform parent)
    {
        gameObject.transform.SetParent(parent);
    }
}
