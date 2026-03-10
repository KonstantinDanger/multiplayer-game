using System.Collections.Generic;
using UnityEngine;

public class SwarmManager : MonoBehaviour
{
    [SerializeField] private List<SwarmEnemy> _swarmMembers = new();

    private void Start()
    {
        _swarmMembers.ForEach(m =>
        {
            m.Initialize(this);
            RegisterSwarmMember(m);
        });
    }

    private void RegisterSwarmMember(SwarmEnemy member)
    {
        if (!_swarmMembers.Contains(member))
            _swarmMembers.Add(member);
    }

    public void UnregisterSwarmMember(SwarmEnemy member)
        => _swarmMembers.Remove(member);

    public void AlertSwarm(Entity target)
    {
        foreach (SwarmEnemy member in _swarmMembers)
        {
            if (member != null && !member.GetComponent<TargetTrackingMemory>().IsTracking)
            {
                member.GetComponent<TargetTrackingMemory>().Memorize(target);
            }
        }
    }
}


