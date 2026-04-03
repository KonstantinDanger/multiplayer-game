using System;
using UnityEngine;

[Serializable]
public class UpgradeInfo
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField, TextArea] public string StatsDescription { get; private set; }
    [field: SerializeField, TextArea] public string RawDescription { get; private set; }

    public string FormattedDescription { get; set; }
}

