using System;
using UnityEngine;

[Serializable]
public class InteractionAction_InvokeUI : InteractionAction
{
    [SerializeField] private Type _uiElementType;

    public override void Act()
    {
        //somehow activate corresponding ui and put it to ui stack
    }
}