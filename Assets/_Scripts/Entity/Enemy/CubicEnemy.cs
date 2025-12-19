using System.Collections;
using UnityEngine;

public class CubicEnemy : DefaultEnemy
{
    [SerializeField, Range(0, 10)] private float _moveInterval = 1.5f;

    private Vector3 _movePosition;

    protected override void OnDamageTaken(Damage damage)
    {
        if (AggroHandler.IsAggroed)
            return;

        base.OnDamageTaken(damage);

        StartCoroutine(ChangeDirectionsRoutine());
    }

    protected override void ChaseTarget(Entity target)
    {
        if (!AggroHandler.IsAggroed)
        {
            StopCoroutine(ChangeDirectionsRoutine());
            return;
        }

        Movable.Move(_movePosition, MovementConfig.Speed);
    }

    private Vector3 ChangeMovePosition()
    {
        Vector3 targetPos = Target.transform.position;

        if (transform.position.y >= targetPos.y + 3 || transform.position.y <= targetPos.y - 3)
        {
            return new Vector3(transform.position.x, targetPos.y, transform.position.z);
        }
        else
        {
            if (transform.position.x >= targetPos.x + 8 || transform.position.x <= targetPos.x - 8)
            {
                return new Vector3(targetPos.x, transform.position.y, transform.position.z);
            }
            else
            {
                if (transform.position.z != targetPos.z)
                    return new Vector3(transform.position.x, transform.position.y, targetPos.z);
            }
        }

        return Vector3.zero;
    }

    private IEnumerator ChangeDirectionsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_moveInterval);
            _movePosition = ChangeMovePosition();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_movePosition, 0.3f);
    }
}
