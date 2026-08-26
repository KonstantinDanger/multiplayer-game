using UnityEngine;

public interface INavigationBasedMover
{
    void Move(Vector3 position, float speed, bool resetVerticalDirection = true, float smoothness = 0.03F);
}
