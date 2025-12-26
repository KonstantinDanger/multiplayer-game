using UnityEngine;

public interface IAttacker
{
    event System.Action OnAttack;

    Transform AttackPoint { get; }
    void PerformAttack();
}
