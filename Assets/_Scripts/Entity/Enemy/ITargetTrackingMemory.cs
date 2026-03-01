using Mirror;

public interface ITargetTrackingMemory
{
    NetworkBehaviour Target { get; }
    void Initialize(TargetTrackingConfig config);
    void Memorize(NetworkBehaviour target);
    void Forget();
}