using Mirror;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class RayCastEndpointAttack : Attack
{
    [SerializeField] private ParticleSystem _particlePrefab;
    [SerializeReference, SubclassSelector] private Attack _attack;

    protected override IEnumerator OnApply(NetworkBehaviour sender, GameObject target)
    {
        var attacker = sender.GetComponentInChildren<IAttacker>();
        var rotatable = sender.GetComponent<IRotatable>();

        Vector3 startPosition = attacker.AttackPoint.position;
        Vector3 direction = rotatable.Forward;
        Vector3 endPos;

        if (Physics.Raycast(startPosition, direction, out RaycastHit hit, Damage.Range, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            endPos = hit.point;
        }
        else
        {
            endPos = startPosition + direction * Damage.Range;
        }

        yield return _attack.Apply(sender);
        sender.StartCoroutine(SpawnParticles(_particlePrefab, endPos));
    }

    private IEnumerator SpawnParticles(ParticleSystem particlePrefab, Vector3 position)
    {
        while (_attack.InProcess)
        {
            var particle = GameObject.Instantiate(particlePrefab, position, Quaternion.identity);
            particle.gameObject.transform.localScale *= _attack.Damage.Range;
            particle.Play(true);
            GameObject.Destroy(particle, 5f);
            yield return new WaitForSeconds(_attack.TimeBetweenAttack);
        }
    }
}

