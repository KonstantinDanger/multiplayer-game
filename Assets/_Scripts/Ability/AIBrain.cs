using Mirror;
using System;
using System.Collections.Generic;

[Serializable]
public class AIBrain : NetworkBehaviour
{
    private List<AIAction> _actions = new();

    public void Initialize(IEnumerable<AIAction> actions)
    {
        _actions = new(actions);

        foreach (var item in _actions)
            item.Initialize(this);
    }

    public void OnUpdate(float deltaTime, NetworkBehaviour target)
    {
        AIAction bestAction = null;
        float highestScore = -1;

        foreach (var action in _actions)
        {
            float currentScore = action.CalculateUtilityScore(this, target);

            if (highestScore < currentScore)
            {
                highestScore = currentScore;
                bestAction = action;
            }
        }

        if (bestAction != null)
        {
            bestAction.Execute(this, target);
        }
        UnityEngine.Debug.Log("Highest score " + highestScore);
    }
}
