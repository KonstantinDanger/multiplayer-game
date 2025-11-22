public interface IAggroHandler
{
    bool IsAggroed { get; }
    Entity CurrentTarget { get; }
    void Aggro(Entity target);
    void Unaggro();
    void OnUpdate(float deltaTime);
    void RefreshAggro();
}
