using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeReference, SubclassSelector] private IInteractionStrategy _interactionStrategy = new InteractionStrategyWithClosest();

    [SerializeField, Range(0.01f, 5f)] private float _interactionRange = 0.5f;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionTime = 0.5f;

    [Header("DEBUG")]
    [SerializeField] private bool _debug;
    //[SerializeField] private Interactions _interactions;

    //public class Interactions
    //{
    //  public InteractionSource Source//Enum of interaction sources (such as: Lever, Npc, ItemPickup etc)
    //  public float InteractionTime;
    //}


    public float InteractionTime => _interactionTime;

    public InteractionInfo Interact()
    {
        List<IInteractable> list = new();

        Collider[] cols = Physics.OverlapSphere(_interactionPoint.position, _interactionRange);

        foreach (Collider col in cols)
            if (col.TryGetComponent(out IInteractable interactable))
                if (!list.Contains(interactable))
                    list.Add(interactable);

        if (list.Count == 0)
            return new InteractionInfo(InteractionResult.Failure, null);

        return _interactionStrategy
            .GetInteractor(list)
            .Interact(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (!_debug)
            return;

        Color color = Color.cyan;
        color.a = 0.2f;
        Gizmos.color = color;
        Gizmos.DrawSphere(_interactionPoint.position, _interactionRange);
    }
}