using Mirror;
using UnityEngine;

public static class EnemyExtensions
{
    /// <summary>
    /// Makes an enemy act like an ally (summon)
    /// </summary>
    /// <param name="entity"></param>
    public static void Summonify(this Enemy enemy, LayerMask layersToDetect, string summonLayerName, NetworkBehaviour owner)
    {
        enemy.TargetDetector.ChangeTargetLayers(layersToDetect);
        enemy.gameObject.layer = LayerMask.NameToLayer(summonLayerName);

        Wallet summonWallet = enemy.GetComponent<Wallet>();
        Wallet ownerWallet = owner.GetComponent<Wallet>();

        if (!summonWallet)
            summonWallet = enemy.gameObject.AddComponent<Wallet>();

        summonWallet.MatchCurrency = new SummonCurrency(ownerWallet.MatchCurrency);

        if (enemy.TryGetComponent(out CurrencyDrop currencyDrop))
            Object.Destroy(currencyDrop);
    }
}
