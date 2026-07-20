using UnityEngine;

public static class EnemyExtensions
{
    /// <summary>
    /// Makes an enemy act like an ally (summon)
    /// </summary>
    /// <param name="entity"></param>
    public static void Summonify(this Enemy enemy, LayerMask layersToDetect, string summonLayerName, Entity owner, bool attacksEveryone = false)
    {
        //enemy.TargetDetector.ChangeTargetLayers(layersToDetect);
        //enemy.gameObject.layer = LayerMask.NameToLayer(summonLayerName);
        enemy.TeamId.Id = owner.TeamId.Id;

        //if (attacksEveryone)
        //    enemy.TeamId.Id += ;

        Wallet summonWallet = enemy.GetComponent<Wallet>();
        Wallet ownerWallet = owner.GetComponent<Wallet>();

        if (!summonWallet)
            summonWallet = enemy.gameObject.AddComponent<Wallet>();

        summonWallet.Initialize(new() { [CurrencyType.Match] = ownerWallet.GetAmountOf(CurrencyType.Match) });

        if (enemy.TryGetComponent(out CurrencyDrop currencyDrop))
            Object.Destroy(currencyDrop);
    }
}
