using UnityEngine;

public class Wallet : MonoBehaviour
{
    public Currency MatchCurrency { get; private set; } = new();
    public Currency StaticCurrency { get; private set; } = new();
}
