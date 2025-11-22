using UnityEngine;

public class SiblingNotifier : MonoBehaviour
{
    [SerializeField] private float notificationRadius = 15f;

    [SerializeField] private bool _debug;

    public void NotifySiblings(Entity attacker)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, notificationRadius);

        foreach (var hit in hits)
        {
            PeacefulEnemy sibling = hit.GetComponent<PeacefulEnemy>();
            if (sibling != null && sibling.gameObject != gameObject)
            {
                sibling.OnSiblingAttacked(attacker);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!_debug)
            return;

        Color col = Color.yellow;
        col.a = 0.1f;
        Gizmos.color = col;
        Gizmos.DrawSphere(transform.position, notificationRadius);
    }
}
