public class GameBootState : GameState
{
    private readonly SceneLoader _sceneLoader;
    private readonly StaticData _staticData;

    public GameBootState(IStateMachine stateMachine, ServiceLocator container) : base(stateMachine, container)
    {
        _sceneLoader = Resolve<SceneLoader>();
        _staticData = Resolve<StaticData>();
    }

    public override void Enter()
        => _sceneLoader.LoadSceneAsync(_staticData.StartingSceneName, OnLoadStarted, OnLoadEnded);

    private void OnLoadStarted() { }
    private void OnLoadEnded() => GoTo<GameMenuState>();
}
