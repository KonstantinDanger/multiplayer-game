using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour, IInteractable
{
    [SerializeField] private float _interactionTime = 2f;

    public bool IsUnlocked;

    public float InteractionTime => _interactionTime;

    public InteractionInfo Interact(GameObject interactor)
    {
        Debug.Log("Checkpoint reached!");
        InteractionInfo interactionInfo = new(InteractionResult.Deny, this);
        IsUnlocked = true;
        return interactionInfo;
    }
}
