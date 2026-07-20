using UnityEngine;

public static class EnemyExtensions
{
    /// <summary>
    /// Makes an enemy act like an ally (summon)
    /// </summary>
    /// <param name="entity"></param>
    public static void Summonify(this Entity entity, Entity owner, bool attacksEveryone = false)
    {
        entity.TeamId.Id = owner.TeamId.Id;

        // Assign another team common for this specie of summon
        // to prevent it from damaging it's kindreds

        //if (attacksEveryone)
        //    enemy.TeamId.Id += ;

        Wallet summonWallet = entity.GetComponent<Wallet>();
        Wallet ownerWallet = owner.GetComponent<Wallet>();

        summonWallet.Initialize(new() { [CurrencyType.Match] = ownerWallet.GetAmountOf(CurrencyType.Match) });

        if (entity.TryGetComponent(out CurrencyDrop currencyDrop))
            Object.Destroy(currencyDrop);
    }
}
