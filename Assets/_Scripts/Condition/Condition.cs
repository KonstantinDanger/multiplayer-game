using Mirror;
using System;

[Serializable]
public abstract class Condition
{
    public abstract bool Fulfilled(NetworkBehaviour sender, NetworkBehaviour target);
}