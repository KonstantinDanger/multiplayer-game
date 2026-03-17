using UnityEngine;

public class Wallet : MonoBehaviour
{
    public Currency MatchCurrency { get; set; } = new();
    public Currency StaticCurrency { get; private set; } = new();
}
