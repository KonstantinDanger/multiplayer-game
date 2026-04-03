using System;
using UnityEngine;

[Serializable]
public class StatUpgrade : Upgrade
{
    [SerializeField] private StatParameter _statParameter;

    protected override void OnPerform(GameObject target)
    {
        EntityStats stats = target.GetComponent<IStatUser>().Stats;

        stats.AddStatMultiplier(_statParameter.Stat, _statParameter.Value);
    }
}


