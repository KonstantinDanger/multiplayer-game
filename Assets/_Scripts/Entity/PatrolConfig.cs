using UnityEngine;

[CreateAssetMenu(menuName = "Config/NavMeshPatrol")]
public class PatrolConfig : ScriptableObject
{
    [SerializeField] private float _patrolRadius = 10f;
    [SerializeField] private float _patrolSpeed = 5f;
    [SerializeField] private float _waitTimeAtPoint = 2f;
    [SerializeField] private float _waypointReachedDistance = 0.5f;
    [SerializeField] private float _rotationSpeed = 20;

    public float PatrolRadius => _patrolRadius;
    public float PatrolSpeed => _patrolSpeed;
    public float WaitTimeAtPoint => _waitTimeAtPoint;
    public float WaypointReachedDistance => _waypointReachedDistance;
    public float RotationSpeed => _rotationSpeed;
}

