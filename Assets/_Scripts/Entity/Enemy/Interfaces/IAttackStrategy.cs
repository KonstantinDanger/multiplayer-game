public interface IAttackStrategy
{
    void ExecuteAttack(Entity target);
    bool CanAttack();
    void ResetCooldown();
    bool IsInAttackRange(Entity target);
}
