using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Config")]
public class EnemyConfig : ScriptableObject
{
    [field: SerializeField, Range(0, 100000)] public int CurrencyDrop { get; private set; }
    [field: SerializeField] public bool IsNeutral { get; private set; }
    [field: SerializeField] public TargetDetectionConfig TargetDetectionConfig { get; private set; }

    [SerializeReference, SubclassSelector] private TargetDetector _targetDetector;

    [SerializeField] private List<ScriptableAIAction> _aiActions = new();

    public IReadOnlyList<AIAction> AIActions
        => _aiActions
        .Select(sA => sA
        .GetNew())
        .ToList();

    public TargetDetector GetNewTargetDetector()
        => Utils.GetInstancedCopyOf(_targetDetector);
}
