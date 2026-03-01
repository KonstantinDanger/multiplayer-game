using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action", fileName = "AIAction_")]
public class ScriptableAIAction : ScriptableObject
{
    [SerializeReference, SubclassSelector] private AIAction _action;

    public AIAction GetNew()
        => Utils.GetInstancedCopyOf(_action);
}
