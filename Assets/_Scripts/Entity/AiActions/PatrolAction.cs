using Mirror;
using System;
using UnityEngine;

[Serializable]
public class PatrolAction : AIAction
{
    [SerializeField] private PatrolConfig _patrolConfig;

    private IPatrol _patrol;

    public override void Initialize(NetworkBehaviour self)
    {
        _patrol = self.GetComponent<IPatrol>();
        _patrol.Initialize(_patrolConfig, self.GetComponent<INavigationBasedMover>(), self.transform, self.transform, self.GetComponent<IRotatable>());
    }

    public override void Execute(Enemy self, NetworkBehaviour target)
        => _patrol.OnUpdate(Time.deltaTime);
}