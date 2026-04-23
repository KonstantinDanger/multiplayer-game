using UnityEngine;

public class ClosedDoor : MonoBehaviour, IInteractable
{
    [field: SerializeField] public float InteractionTime { get; private set; }
    [SerializeReference, SubclassSelector] private DoorUnlockCondition _doorUnlockCondition;
    [SerializeReference, SubclassSelector] private IOpenable _doorOpenStrategy;

    public InteractionInfo Interact(GameObject interactor)
    {
        if (!_doorUnlockCondition.Fulfilled(interactor))
            return new(InteractionResult.Deny, this);

        Open();

        return new(InteractionResult.Success, this);
    }

    public void Open()
        => _doorOpenStrategy.Open();
}