using System;

[Serializable]
public class StaggerEffect : StatusEffect
{
    // duration

    // interrupts an attack 
    // stuns for a brief moment

    protected override void Apply(Entity entity)
    {
        //entity.GetComponent<IAbilityUser>().Cancel();

    }
}


