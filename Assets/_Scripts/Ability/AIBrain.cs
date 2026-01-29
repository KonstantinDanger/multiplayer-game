using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AIBrain : NetworkBehaviour
{
    [SerializeField] private List<AIAction> _actions = new();

    private NetworkBehaviour _target;

    private void Awake()
    {
        foreach (var action in _actions)
            action?.Initialize(this);
    }

    private void Update()
    {
        AIAction bestAction = null;
        float highestScore = -1;

        foreach (var action in _actions)
        {
            float currentScore = action.CalculateUtilityScore(this, _target);

            if (highestScore < currentScore)
            {
                highestScore = currentScore;
                bestAction = action;
            }
        }

        if (bestAction != null)
        {
            bestAction.Execute(this, _target);
        }
    }
}
