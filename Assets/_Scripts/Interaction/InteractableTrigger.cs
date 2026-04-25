using UnityEngine;
using UnityEngine.Events;

public class InteractableTrigger : MonoBehaviour, IInteractable
{
    [field: SerializeField, Range(0f, 100f)] public float InteractionTime { get; private set; } = 1f;

    public UnityEvent OnTriggered;

    public InteractionInfo Interact(GameObject interactor)
    {
        OnTriggered?.Invoke();

        return new InteractionInfo(InteractionResult.Success, this);
    }
}
