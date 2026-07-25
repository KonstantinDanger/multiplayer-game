using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ParryAbility : Ability
{
    [SerializeReference, SubclassSelector] private TargetDetector _targetDetector;

    [SerializeField, Range(0f, 5f)] private float _parryTime = 0.08f;
    [SerializeField, Range(0f, 10f)] private float _parryRadius = 0.5f;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        // Negate damage
        // (Stun or Stagger target) - melee OR 
        // Apply knockback

        sender.StartCoroutine(ParryRoutine(sender));

        yield return null;
    }

    private IEnumerator ParryRoutine(NetworkBehaviour sender)
    {
        yield return new WaitForSeconds(UsagePrepareTime);

        float elapsedTime = 0f;

        while (elapsedTime < _parryTime)
        {
            elapsedTime += Time.deltaTime;

            Parry(sender);

            yield return null;
        }
    }

    private bool Parry(NetworkBehaviour sender)
    {
        Vector3 origin = GetDetectionOriginFrom(sender).position;
        Vector3 direction = sender.GetComponent<IRotatable>().Forward;

        _targetDetector.DetectAllTargets(sender.gameObject, origin, direction, _parryRadius, out List<GameObject> targets);

        var parryTarget = targets.SortByDistance(sender.gameObject).FirstOrDefault();

        if (parryTarget == null || sender == null)
            return false;

        if (!parryTarget.TryGetComponent(out IParryable parryable))
            return false;

        parryable.ReactTo(sender);

        return true;
    }

    private Transform GetDetectionOriginFrom(NetworkBehaviour sender)
        => sender.TryGetComponent(out IAttacker attacker)
            ? attacker.AttackPoint
            : sender.transform;
}
