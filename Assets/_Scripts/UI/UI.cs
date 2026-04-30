using UnityEngine;

public class UI : MonoBehaviour
{
    [field: SerializeField] public bool PersistentThroughScenes { get; private set; } = true;
}
