using System;
using TypeReferences;
using UnityEngine;

public class InteractionAction_InvokeUI : InteractionAction
{
    [SerializeField, Inherits(typeof(UIView))]
    private TypeReference _uiTypeReference;

    public override void Act()
    {
        GameUI ui = ServiceLocator.Container.Resolve<GameUI>();

        Type type = _uiTypeReference.Type;

        if (!typeof(UIView).IsAssignableFrom(type))
            throw new Exception($"\"Type {type}\" does not inherit from UIView type");

        ui.OpenViewOfType(type);
    }
}