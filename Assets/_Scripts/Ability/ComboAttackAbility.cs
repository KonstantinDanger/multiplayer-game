using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComboAttackAbility : Ability
{
    [SerializeReference, SubclassSelector] private List<Attack> _attacks = new();
    [SerializeField, Range(0f, 1f)] private float _nextAttackCacheProgressThreshold = 0.5f;
    [SerializeField, Range(0f, 5)] private float _comboCooldownTime = 0.3f;

    private int _currentAttackIndex = -1;

    private Attack CurrentAttack => _attacks[_currentAttackIndex];

    private Attack NextAttack
    {
        get
        {
            if (_currentAttackIndex >= _attacks.Count - 1)
                _currentAttackIndex = 0;
            else
                _currentAttackIndex++;

            return _attacks[_currentAttackIndex];
        }
    }

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        if (sender == null)
            yield break;

        NextAttack.Apply(sender, target);

        // if attack progress < _nextAttackCacheProgressThreshold : return false
        // else if attack is ended : return true
        //     else : cache next attack

        // if combo is ended : SetNextAttackTime => _comboCooldownTime;

        yield break;
    }
}
