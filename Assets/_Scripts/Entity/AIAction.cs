using Mirror;
using UnityEngine;

public abstract class AIAction : ScriptableObject
{
    [SerializeReference, SubclassSelector] private Consideration _consideration;
    public virtual void Initialize(NetworkBehaviour self) { }
    public abstract void Execute(NetworkBehaviour self, NetworkBehaviour target);
    public float CalculateUtilityScore(NetworkBehaviour self, NetworkBehaviour target) => _consideration.Evaluate(self, target);
}
