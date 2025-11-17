using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Config")]
public class EnemyConfig : ScriptableObject
{
    [field: SerializeField, Range(0f, 100000f)] public float XpToDrop { get; private set; }
}

