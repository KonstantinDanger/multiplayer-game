using Mirror;

public interface ITargetTrackingMemory
{
    public bool IsTracking { get; }
    NetworkBehaviour Target { get; }
    void Memorize(NetworkBehaviour target);
    void Forget();
}