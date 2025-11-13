using System;
using UnityEngine;

[Serializable]
public class AttackAbility : Ability
{
    protected override void OnPerform(GameObject sender, GameObject target)
        => UnityEngine.Debug.Log("Attack performed ");
}
