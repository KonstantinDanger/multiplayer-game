using Mirror;
using System.Collections.Generic;

public class AIBrain : NetworkBehaviour
{
    private List<AIAction> _actions = new();
    private Enemy _self;

    public void Initialize(Enemy enemy, IEnumerable<AIAction> actions)
    {
        _actions = new(actions);

        foreach (var item in _actions)
            item.Initialize(this);

        _self = enemy;
    }

    public void OnUpdate(float deltaTime, NetworkBehaviour target)
    {
        AIAction bestAction = null;
        float highestScore = -1;

        foreach (var action in _actions)
        {
            float currentScore = action.CalculateUtilityScore(_self, target);

            if (highestScore < currentScore)
            {
                highestScore = currentScore;
                bestAction = action;
            }
        }

        if (bestAction != null)
        {
            bestAction.Execute(_self, target);
        }
    }
}
