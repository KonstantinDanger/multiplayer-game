using UnityEngine;

public interface IFleeBehaviour
{
    Vector3 GetFleeDestinationFrom(Transform attacker, Transform self, float fleeDistance);
}
