using Mirror;
using System.Collections.Generic;
using System.Linq;

public class AIBrain : NetworkBehaviour
{
    private List<AIAction> _actions = new();
    private Enemy _self;

    private AIAction _currentAction = null;

    public void Initialize(Enemy enemy, IEnumerable<AIAction> actions)
    {
        if (actions.Count() == 0)
        {
            UnityEngine.Debug.LogError("No actions found!");
            return;
        }

        _actions = new(actions);

        foreach (var item in _actions)
            item.Initialize(this);

        _self = enemy;
    }

    public void OnUpdate(float deltaTime, NetworkBehaviour target)
    {
        if (_currentAction != null && _currentAction.IsLocked)
        {
            _currentAction.Execute(_self, target);
            return;
        }

        AIAction nextAction = FindBestAction(_self, target);

        if (_currentAction != nextAction)
        {
            _currentAction?.Exit(_self, target);
            _currentAction = nextAction;
            _currentAction.Enter(_self, target);
        }

        _currentAction.Execute(_self, target);


        UnityEngine.Debug.Log("Current action: " + _currentAction);
    }

    private AIAction FindBestAction(Enemy self, NetworkBehaviour target)
    {
        AIAction bestAction = null;
        float highestScore = -1;

        foreach (var action in _actions)
        {
            float currentScore = action.CalculateUtilityScore(self, target);

            if (highestScore < currentScore)
            {
                highestScore = currentScore;
                bestAction = action;
            }
        }

        return bestAction;
    }
}
