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

        // 1. view adds itself to the game ui stack
        // 2. we get game ui ref
        // 3. get view subclass type
        // 4. call method "OpenViewByType"

        Type type = _uiTypeReference.Type;

        if (!typeof(UIView).IsAssignableFrom(type))
            throw new Exception($"\"Type {type}\" does not inherit from UIView type");

        ui.OpenViewOfType(type);
    }
}