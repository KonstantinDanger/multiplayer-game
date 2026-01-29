using System.Collections.Generic;
using UnityEngine;

public class AIAbilityHandler : AbilityHandler
{
    [SerializeField] private List<Consideration> _considerations = new();

    public AIAbilityHandler(Ability ability) : base(ability) { }
}
