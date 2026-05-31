using System;

public class Timer
{
    private readonly float _targetTime = 0f;

    private Action OnEnded;
    private float _elapsedTime = 0f;
    private bool _isGoing;

    public bool IsEnded => _elapsedTime >= _targetTime;
    public float ElapsedTime => _elapsedTime;
    public bool HasStarted { get; private set; }

    public Timer(float targetTime)
    {
        _elapsedTime = 0f;
        _targetTime = targetTime;
    }

    public void Start(Action onEnded = null)
    {
        OnEnded = onEnded;

        _isGoing = true;

        HasStarted = true;
    }

    public void Update(float deltaTime)
    {
        if (!_isGoing)
            return;

        _elapsedTime += deltaTime;

        if (IsEnded)
        {
            _isGoing = false;
            OnEnded?.Invoke();
        }
    }

    public void Stop()
    {
        _isGoing = false;
        _elapsedTime = 0f;

    }
}

