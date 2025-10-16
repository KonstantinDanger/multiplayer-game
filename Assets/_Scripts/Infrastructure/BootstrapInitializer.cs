using UnityEngine;

public class BootstrapInitializer : MonoBehaviour
{
    [SerializeField] private StaticData _staticData;

    private void Awake()
        => DontDestroyOnLoad(this);

    private void Start()
        => BindServices();

    private void BindServices()
    {
        SceneLoader sceneLoader = new(this);
        GameFactory factory = new();

        ServiceLocator.Container.RegisterSingle(sceneLoader);
        ServiceLocator.Container.RegisterSingle(_staticData);
        ServiceLocator.Container.RegisterSingle(factory);

        sceneLoader.LoadScene(_staticData.StartingSceneName);
    }
}