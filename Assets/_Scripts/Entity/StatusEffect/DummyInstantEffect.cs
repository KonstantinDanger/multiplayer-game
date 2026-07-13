using System;

[Serializable]
public class DummyInstantEffect : InstantStatusEffect
{
    public override void Proc(Entity entity) => UnityEngine.Debug.Log("Some instant status effect has been applied ");
}


