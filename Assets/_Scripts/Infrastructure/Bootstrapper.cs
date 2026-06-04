using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private CustomNetworkManager _netManager;
    [SerializeField] private Game _gamePrefab;

    private void Start()
    {
        GameObject persistentObject = new GameObject("PersistentObject");

        DontDestroyOnLoad(persistentObject);

        var netManager = Instantiate(_netManager, persistentObject.transform);
        var game = Instantiate(_gamePrefab, persistentObject.transform);

        game.Initialize(netManager);

        ServiceLocator.Container.RegisterSingle(game);
    }
}
