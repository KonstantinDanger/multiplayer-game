using AYellowpaper;
using System;
using UnityEngine;

[RequireComponent(typeof(IMovable))]
public class NavMeshDirectionChanger : MonoBehaviour, INavMeshDirectionChanger
{
    [SerializeField] private InterfaceReference<IMovable> _movableRef;
    [SerializeField] private UnityEngine.AI.NavMeshAgent _agent;


}

public interface INavMeshDirectionChanger { }

public interface IMovable
{
    event Action OnMove;
    bool IsGrounded { get; }
    bool IsGravityActive { get; set; }
    Vector3 Velocity { get; }
    Vector3 MoveDirection { get; }
    void ResetVerticalVelocity();
    void ApplyGravity(float gravity, float maxFallSpeed);
    void Jump(float jumpHeight, float gravity);
    void AddExternalForce(Vector3 force);
    void UpdateExternalForce(float drag);
    void Warp(Vector3 position);
    void Move(Vector3 position, float speed, bool resetVerticalDirection = true, float smoothness = 0.03F);
}
