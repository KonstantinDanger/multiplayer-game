using Mirror;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class ParryAbility : Ability
{
    [SerializeReference, SubclassSelector] private ITargetDetector _targetDetector;

    [SerializeField, Range(0f, 5f)] private float _parryTime = 0.08f;
    [SerializeField, Range(0f, 10f)] private float _parryRadius = 0.5f;
    [SerializeField, Range(0f, 720f)] private float _parryAngle = 360;

    protected override bool OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        // Negate damage
        // (Stun or Stagger target) - melee OR 
        // Apply knockback

        sender.StartCoroutine(ParryRoutine(sender));

        return true;
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
        _targetDetector.Origin = GetDetectionOriginFrom(sender);

        var parryTarget = _targetDetector.DetectNearestTarget(_parryRadius, _parryAngle);

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
