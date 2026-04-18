using UnityEngine;

public class Map : MonoBehaviour
{
    [field: SerializeField] public Transform MapCenter { get; private set; }

    private void Awake()
        => ServiceLocator.Container.RegisterSingle(this, cached: true);
}
