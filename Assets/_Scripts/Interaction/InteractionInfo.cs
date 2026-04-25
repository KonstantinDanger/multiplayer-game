public readonly struct InteractionInfo
{
    public readonly InteractionResult Result;
    public readonly IInteractable Interactable;

    public InteractionInfo(InteractionResult result, IInteractable interactable)
    {
        Result = result;
        Interactable = interactable;
    }
}