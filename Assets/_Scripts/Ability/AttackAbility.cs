using Mirror;
using System;
using UnityEngine;

[Serializable]
public class AttackAbility : Ability
{
    [SerializeReference, SubclassSelector] private Attack _attack;

    protected override void OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        _attack.Apply(sender, target);

        {//UnityEngine.Debug.Log($"Ability \'{Name}\' performed");
        }
    }
}
