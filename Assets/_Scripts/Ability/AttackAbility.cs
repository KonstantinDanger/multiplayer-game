using System;
using UnityEngine;

[Serializable]
public class AttackAbility : Ability
{
    [SerializeReference, SubclassSelector] private Attack _attack;

    protected override void OnPerform(GameObject sender, GameObject target)
    {
        _attack.Apply(sender, target);

        {//UnityEngine.Debug.Log($"Ability \'{Name}\' performed");
        }
    }
}
