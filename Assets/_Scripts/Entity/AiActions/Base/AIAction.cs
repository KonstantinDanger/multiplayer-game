using Mirror;
using System;
using UnityEngine;

[Serializable]
public abstract class AIAction
{
    [SerializeReference, SubclassSelector] private Consideration _consideration;

    public abstract void Execute(Enemy self, NetworkBehaviour target);
    public float CalculateUtilityScore(Enemy self, NetworkBehaviour target) => _consideration.Evaluate(self, target);
    public virtual void Initialize(NetworkBehaviour self) { }
}
