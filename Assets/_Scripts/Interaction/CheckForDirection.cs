using System;
using UnityEngine;

[Serializable]
public class CheckForDirection : DoorUnlockCondition
{
    [SerializeField] private Transform _openPoint;
    [SerializeField] private Transform _transform;

    public override bool Fulfilled(GameObject interactor)
    {
        Vector3 openDirection = _transform.position - _openPoint.position;
        Vector3 directionFromInteractor = _transform.position - interactor.transform.position;

        bool isBehindTheDoor = Vector3.Angle(openDirection, directionFromInteractor) < 90f;

        return isBehindTheDoor;
    }
}
