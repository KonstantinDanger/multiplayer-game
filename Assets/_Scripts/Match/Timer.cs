public class Timer
{
    private readonly float _targetTime = 0f;

    private float _elapsedTime = 0f;

    private bool _isGoing;

    public bool IsEnded => _elapsedTime >= _targetTime;
    public bool HasStarted => _elapsedTime == 0f;
    public float ElapsedTime => _elapsedTime;

    public Timer(float targetTime)
    {
        _elapsedTime = 0f;
        _targetTime = targetTime;
    }

    public void Start()
        => _isGoing = true;

    public void Update(float deltaTime)
    {
        if (!_isGoing)
            return;

        _elapsedTime += deltaTime;

        if (IsEnded)
        {
            _isGoing = false;
        }
    }
}

