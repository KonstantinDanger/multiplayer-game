using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntitySummonAbility : Ability
{
    [SerializeField, Range(1, 20)] private int _maxSummonCount;
    [SerializeField] private Entity _entityPrefab;

    private readonly List<Entity> _summonInstances = new();

    protected override void OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (_summonInstances.Contains(null))
        {
            for (int i = 0; i < _summonInstances.Count; i++)
            {
                if (_summonInstances[i] == null)
                    _summonInstances.RemoveAt(i);
            }
        }

        if (_summonInstances.Count == _maxSummonCount)
        {
            NetworkServer.Destroy(_summonInstances[0].gameObject);
            _summonInstances.RemoveAt(0);
        }

        GameFactory factory = ServiceLocator.Container.Resolve<GameFactory>();

        //Summon entity at look position if there's ground
        //Else: Summon entity close by

        IRotatable rotatable = sender.GetComponent<IRotatable>();

        //Temporary decision:
        Vector3 summonPosition = sender.transform.position;

        Vector3 forwardDirection = rotatable.Forward;
        forwardDirection.y = 0f;

        Quaternion rotation = Quaternion.LookRotation(forwardDirection);
        //Temporary decision.

        var instance = factory.SpawnEntity(_entityPrefab, summonPosition, rotation, owner: sender);
        _summonInstances.Add(instance);
    }
}
