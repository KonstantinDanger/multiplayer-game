using System.Collections.Generic;
using System.Linq;

public class InteractionStrategyWithClosest : IInteractionStrategy
{
    public IInteractable GetInteractor(IEnumerable<IInteractable> objects)
    {
        return objects?.First();
    }
}