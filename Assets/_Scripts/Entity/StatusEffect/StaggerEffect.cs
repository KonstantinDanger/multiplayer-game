using System;

[Serializable]
public class StaggerEffect : ProlongedStatusEffect
{
    // duration

    // interrupts an attack 
    // stuns for a brief moment

    public override void Proc(Entity entity)
    {
        //entity.GetComponent<IAbilityUser>().Cancel();
    }
}


