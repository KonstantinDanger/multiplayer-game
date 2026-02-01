using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Config")]
public class EnemyConfig : ScriptableObject
{
    [field: SerializeField, Range(0f, 100000f)] public float XpToDrop { get; private set; }
    [field: SerializeField] public bool IsNeutral { get; private set; }
    [field: SerializeField] public float FieldOfViewAngle { get; private set; } = 120f;
    [field: SerializeField] public float VisionRange { get; private set; } = 15f;

    [field: SerializeField] public float DetectionRadius { get; private set; } = 10f;
    [field: SerializeField] public float DetectionInterval { get; private set; } = 5f; //Probably remove it to static field
    [SerializeField] private List<ScriptableAIAction> _aiActions = new();

    public IReadOnlyList<AIAction> AIActions
        => _aiActions
        .Select(sA => sA
        .GetNew())
        .ToList();
}

