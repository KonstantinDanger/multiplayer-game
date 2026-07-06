[System.Serializable]
public abstract class DamageHandler
{
    /// <summary>
    /// Returns false if damage is negated
    /// </summary>
    /// <returns></returns>
    public abstract bool Calculate(Damage damage, out Damage result);
}
