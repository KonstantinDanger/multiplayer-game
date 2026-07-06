using System.Collections;
using UnityEngine;

[System.Serializable]
public sealed class StaggerDamageHandler : DamageHandler
{
    [SerializeField, Range(0, 1000)] private float _damageToStagger;

    private float _damageDealt = 0f;

    public override bool Calculate(Damage damage, out Damage result)
    {
        result = damage;

        _damageDealt += damage.Amount;

        if (_damageDealt >= _damageToStagger)
        {
            float damageRemainder = _damageDealt - _damageDealt;
            //perform stagger
            _damageDealt = damageRemainder;
        }

        return true;
    }
}

[System.Serializable]
public sealed class StunDamageHandler : DamageHandler
{
    [SerializeField, Range(0, 1000)] private float _damageToStun;
    [SerializeField, Range(0, 60)] private float _regenDelay;
    [SerializeField, Range(0, 60)] private float _regenSpeed;
    [SerializeField, Range(0, 60)] private float _regenSpeedAfterStanceBreak;

    private float _damageDealt = 0f;

    public override bool Calculate(Damage damage, out Damage result)
    {
        result = damage;



        return true;
    }

    private void Regen()
    {

    }

    private IEnumerator RegenStanceRoutine()
    {


        yield return new WaitForSeconds(_regenDelay);

        while (_damageDealt > 0)
        {
            _damageDealt -= Time.deltaTime * _regenSpeed;
        }
    }
}

