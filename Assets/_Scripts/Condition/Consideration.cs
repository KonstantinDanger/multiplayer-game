using Mirror;
using System;

[Serializable]
public abstract class Consideration
{
    public abstract float Evaluate(NetworkBehaviour sender, NetworkBehaviour target);
}
