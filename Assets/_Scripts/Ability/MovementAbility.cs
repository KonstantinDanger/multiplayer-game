using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class MovementAbility : Ability
{
    [SerializeField] private float _warpDistance = 10;
    [SerializeField] private float _warpTime = 0.2f;

    protected override void OnPerform(GameObject sender, GameObject target)
    {
        IRotatable rotatable = sender.GetComponent<IRotatable>();
        IMovable movable = sender.GetComponent<IMovable>();

        var coroutineHolder = ServiceLocator.Container.Resolve<CoroutineHolder>();

        coroutineHolder.StartCoroutine(WarpRoutine(sender, movable, rotatable));
    }

    private IEnumerator WarpRoutine(GameObject sender, IMovable movable, IRotatable rotatable)
    {
        float warpEndTime = Time.time + _warpTime;

        Vector3 startPosition = sender.transform.position;
        Vector3 forward = rotatable.Forward;

        movable.IsGravityActive = false;

        while (Time.time < warpEndTime)
        {
            Vector3 moveDirection = forward.normalized * _warpDistance;

            Vector3 movementDelta = Vector3.Lerp(startPosition, moveDirection, Time.time / warpEndTime);

            movable.Move(movementDelta, _warpDistance / _warpTime);

            yield return null;
        }

        movable.IsGravityActive = true;
        movable.ResetVerticalVelocity();
    }
}
