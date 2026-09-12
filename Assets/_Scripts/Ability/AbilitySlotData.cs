public struct AbilitySlotData
{
    public int AccumulatedCharges;
    public int MaxCharges;

    /// <summary>
    /// Value should be in range: [0.0, 1.0]
    /// </summary>
    public float RechargeProgress;

    public override readonly string ToString()
        => $"Slot data: Accumulation {AccumulatedCharges} | max charges {MaxCharges} | recharge progress {RechargeProgress}";
}
