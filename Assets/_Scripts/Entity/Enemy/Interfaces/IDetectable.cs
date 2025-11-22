using System.Collections.Generic;

public interface IDetectable<T> where T : Entity
{
    T DetectNearestPlayer(float detectionRadius);
    List<T> DetectAllPlayers(float detectionRadius);
    bool IsInFieldOfView(Entity target, float fovAngle, float maxDistance);
}
