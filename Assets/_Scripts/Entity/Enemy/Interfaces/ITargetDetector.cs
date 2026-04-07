using UnityEngine;

public interface ITargetDetector
{
    void ChangeTargetLayers(LayerMask layersToDetect);
    Entity DetectNearestTarget(float detectionRadius);
    bool IsInFieldOfView(Entity target, float fovAngle, float maxDistance);
}
