using System.Collections.Generic;

public interface IInteractionStrategy
{
    public IInteractable GetInteractor(IEnumerable<IInteractable> objects);
}