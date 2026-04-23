using UnityEngine;
using UnityEngine.Events;

public class InteractableTrigger : MonoBehaviour, IInteractable
{
    [field: SerializeField] public float InteractionTime { get; private set; }

    public UnityEvent OnTriggered;

    public InteractionInfo Interact(GameObject interactor)
    {
        OnTriggered?.Invoke();

        return new InteractionInfo(InteractionResult.Success, this);
    }
}
