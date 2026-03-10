public interface ITargetDetector
{
    Entity DetectNearestTarget(float detectionRadius);
    bool IsInFieldOfView(Entity target, float fovAngle, float maxDistance);
}
