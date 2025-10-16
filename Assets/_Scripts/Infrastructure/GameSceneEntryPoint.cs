using UnityEngine;

public class GameSceneEntryPoint : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint;

    protected Transform PlayerSpawnPoint => _playerSpawnPoint;
    protected GameFactory GameFactory { get; private set; }
    protected StaticData StaticData { get; private set; }

    private void Start()
    {
        GameFactory = ServiceLocator.Container.Resolve<GameFactory>();
        StaticData = ServiceLocator.Container.Resolve<StaticData>();

        OnStart();
    }

    public virtual void OnStart() { }
}