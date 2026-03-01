using UnityEngine;

[CreateAssetMenu(menuName = "AI/Action", fileName = "AIAction_")]
public class ScriptableAIAction : ScriptableObject
{
    [SerializeReference, SubclassSelector] private AIAction _action;

    public AIAction GetNew()
    {
        if (_action == null)
            return null;

        string json = JsonUtility.ToJson(_action);
        AIAction clone = JsonUtility.FromJson(json, _action.GetType()) as AIAction;

        return clone;
    }
}
