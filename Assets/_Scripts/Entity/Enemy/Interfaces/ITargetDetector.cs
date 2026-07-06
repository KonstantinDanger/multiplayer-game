using UnityEngine;

public interface ITargetDetector
{
    Transform Origin { get; set; }

    GameObject DetectNearestTarget(float detectionRadius, float fieldOfViewAngle);
    bool IsInFieldOfView(GameObject target, float fovAngle);
    void ChangeTargetLayers(LayerMask layersToDetect);
}
