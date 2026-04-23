using System;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private enum State { Idle, Going }

    [SerializeField] private ElevatorPlatform _elevatorPlatform;

    [Header("Floor Points")]
    [SerializeField] private List<Transform> _floorPoints = new();
    [SerializeField] private Transform _startingFloorPoint;
    [SerializeField] private Transform _finalDestinationFloorPoint;

    [SerializeField] private float _movementSpeed = 5f;

    private Transform _currentFloorPoint;

    private bool _isStandingOnThePlate;

    private State _state = State.Idle;

    private void OnEnable()
    {
        var pressurePlate = _elevatorPlatform.PressurePlate;

        pressurePlate.OnTriggerEntered += OnPlateTriggered;
        //pressurePlate.OnTriggerStayed += _ => _isStandingOnThePlate = true;
        pressurePlate.OnTriggerExited += _ => _isStandingOnThePlate = false;
    }

    private void OnDisable()
    {
        var pressurePlate = _elevatorPlatform.PressurePlate;

        pressurePlate.OnTriggerEntered -= OnPlateTriggered;
        //pressurePlate.OnTriggerStayed -= _ => _isStandingOnThePlate = true;
        pressurePlate.OnTriggerExited -= _ => _isStandingOnThePlate = false;
    }

    public void GoToFloor(int floorIndex)
    {
        if (_state == State.Going)
            return;

        if (floorIndex <= 0 || floorIndex > _floorPoints.Count)
            throw new Exception($"Invalid floor index: {floorIndex}");

        _currentFloorPoint = _floorPoints[floorIndex - 1];

        StartCoroutine(LiftRoutine(_currentFloorPoint));
    }

    private void OnPlateTriggered(Collider other)
    {
        if (!CanLift(other))
            return;

        _isStandingOnThePlate = true;
        int index = GetFloorPointToGoIndex();
        GoToFloor(index);
    }

    private bool CanLift(Collider other)
    {
        return IsEntity(other.gameObject) &&
            !_isStandingOnThePlate &&
            _state == State.Idle;
    }

    private int GetFloorPointToGoIndex()
    {
        return (_currentFloorPoint == null || _currentFloorPoint == _startingFloorPoint) ?
                _floorPoints.Count : 1;
    }

    private bool IsEntity(GameObject gameObject)
    {
        return gameObject.TryGetComponent(out Entity entity);
    }

    private System.Collections.IEnumerator LiftRoutine(Transform floorPointTransform)
    {
        _state = State.Going;

        Vector3 targetPosition = floorPointTransform.position;

        const float MinStoppingDistance = 0.01f;

        while (Vector3.Distance(_elevatorPlatform.Pivot.position, targetPosition) > MinStoppingDistance)
        {
            float currentDelta = _movementSpeed * Time.deltaTime;
            _elevatorPlatform.transform.position = Vector3.MoveTowards(_elevatorPlatform.transform.position, targetPosition, currentDelta);

            yield return null;
        }

        _state = State.Idle;
    }
}
