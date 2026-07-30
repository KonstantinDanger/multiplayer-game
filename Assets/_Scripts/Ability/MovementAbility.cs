using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class MovementAbility : Ability
{
    [Header("If set to false - direction will be forward from rotatable")]
    [SerializeField] private bool _directional;
    [SerializeField] private bool _resetVerticalDirection = true;
    [SerializeField] private float _warpDistance = 10;
    [SerializeField] private float _warpTime = 0.2f;

    public override float Duration => _warpTime;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        IRotatable rotatable = sender.GetComponent<IRotatable>();
        IMovable movable = sender.GetComponent<IMovable>();

        yield return MoveRoutine(sender.gameObject, movable, rotatable);
    }

    private IEnumerator MoveRoutine(GameObject sender, IMovable movable, IRotatable rotatable)
    {
        float warpTimeEnd = Time.time + _warpTime;
        float moveSpeed = _warpDistance / _warpTime;

        Vector3 moveDir = _directional ? movable.MoveDirection : rotatable.Forward;
        moveDir = moveDir == Vector3.zero ? sender.transform.forward : moveDir;
        moveDir.Normalize();

        movable.IsGravityActive = false;

        while (Time.time < warpTimeEnd)
        {
            movable.Move(moveDir, moveSpeed, _resetVerticalDirection, 0f);

            yield return null;
        }

        movable.IsGravityActive = true;
        movable.ResetVerticalVelocity();
    }
}
