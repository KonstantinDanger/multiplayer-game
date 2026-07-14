using System;

[Serializable]
public class StaggerEffect : ProlongedStatusEffect
{
    // duration

    // interrupts an attack 
    // stuns for a brief moment

    protected override void OnProc(Entity entity)
    {
        //entity.GetComponent<IAbilityUser>().Cancel();
    }
}
