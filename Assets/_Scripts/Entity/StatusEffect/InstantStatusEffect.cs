[System.Serializable]
public abstract class InstantStatusEffect : StatusEffect
{
    protected sealed override void OnDecay(float deltaTime, float decaySpeed)
        => base.OnDecay(deltaTime, decaySpeed);

    protected sealed override void OnTick(float deltaTime)
        => base.OnTick(deltaTime);
}
