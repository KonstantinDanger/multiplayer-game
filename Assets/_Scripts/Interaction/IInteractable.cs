using UnityEngine;

public interface IInteractable
{
    public float InteractionTime { get; }
    InteractionInfo Interact(GameObject interactor);
}