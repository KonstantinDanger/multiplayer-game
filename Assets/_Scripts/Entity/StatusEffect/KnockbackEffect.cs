using System;

[Serializable]
public class KnockbackEffect : ProlongedStatusEffect
{
    // time = this.Duration
    // distance

    // moves IMovable by distance over time
    protected override void OnProc(Entity entity) => throw new NotImplementedException();
}