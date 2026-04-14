using UnityEngine;

public interface ITargetDetector
{
    void ChangeTargetLayers(LayerMask layersToDetect);
    Entity DetectNearestTarget(float detectionRadius, float fieldOfViewAngle);
    bool IsInFieldOfView(Entity target, float fovAngle);
}
