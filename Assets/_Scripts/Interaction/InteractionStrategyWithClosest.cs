using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class InteractionStrategyWithClosest : IInteractionStrategy
{
    public IInteractable GetInteractor(IEnumerable<IInteractable> objects)
        => objects?.First();
}