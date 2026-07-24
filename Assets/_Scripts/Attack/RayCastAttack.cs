public class RayCastAttack
{
    //private void DrawRay(Vector3 startPosition, Vector3 direction, RayCastView rayCastView)
    //{
    //    Vector3 endPos = startPosition + direction * Damage.Range;
    //    Vector3 attackPosition = startPosition /*+ currentVelocity * Time.deltaTime*/;

    //    rayCastView.StartBulletView(attackPosition, endPos);
    //}

    //private void TryDamageTarget(NetworkBehaviour target, RaycastHit hit, int penetrationCount)
    //{
    //    if (_sender == hit.collider.gameObject)
    //        return;

    //    if (!hit.collider.TryGetComponent(out IDamageable damageable))
    //        return;

    //    float damageDecayMultiplier = 1 - Mathf.Clamp(penetrationCount - 1, 0f, _penetrationCount) * (_damageDecayPercentOverPenetration / 100);
    //    damageDecayMultiplier = Mathf.Clamp01(damageDecayMultiplier);

    //    var damage = SetupDamage(Damage, Damage.Sender);
    //    damage.Amount *= damageDecayMultiplier;
    //    damageable.TakeDamage(damage);
    //}

    //private Damage SetupDamage(Damage baseDamage, GameObject sender)
    //{
    //    if (!sender.TryGetComponent(out EntityStats stats))
    //        return baseDamage;

    //    Damage damage = baseDamage;
    //    damage.Amount *= stats.GetStatMultiplier(StatType.Damage);

    //    return damage;
    //}
}

