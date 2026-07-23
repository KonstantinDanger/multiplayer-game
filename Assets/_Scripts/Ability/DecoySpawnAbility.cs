using Mirror;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class DecoySpawnAbility : Ability
{
    [SerializeField, Range(1, 20)] private int _decoyAmount = 2;
    [SerializeField, Range(1f, 120f)] private float _decoyLifetime = 5f;
    [SerializeField, Range(0f, 20f)] private float _spawnPositionSpreadX = 5f;
    [SerializeField, Range(0f, 20f)] private float _spawnPositionSpreadY = 5f;
    [SerializeField, Range(0f, 20f)] private float _spawnPositionSpreadZ = 5f;
    [SerializeField, Range(0f, 360f)] private float _maxRandomRotationRange = 90f;
    [SerializeField] private bool _swapWithClone = true;

    protected override IEnumerator OnPerform(NetworkBehaviour sender, NetworkBehaviour target)
    {
        var factory = ServiceLocator.Container.Resolve<GameFactory>();

        Vector3 senderPos = sender.transform.position;
        Quaternion senderRot = sender.transform.rotation;

        for (int i = 0; i < _decoyAmount; i++)
        {
            float x = GetRndCoordBetween(-_spawnPositionSpreadX, _spawnPositionSpreadX);
            float y = GetRndCoordBetween(-_spawnPositionSpreadY, _spawnPositionSpreadY);
            float z = GetRndCoordBetween(-_spawnPositionSpreadZ, _spawnPositionSpreadZ);

            Vector3 position = new Vector3(senderPos.x + x,
                                           senderPos.y + y,
                                           senderPos.z + z);

            float rotationRange = _maxRandomRotationRange / 2;
            Quaternion rotation = Quaternion.Euler(senderRot.x, senderRot.y + Random.Range(-rotationRange, rotationRange), senderRot.z);

            if (_swapWithClone && i == 0)
            {
                sender.TryGetComponent(out IMovable movable);
                movable.Warp(position);
                position = senderPos;
            }

            factory.SpawnDecoy(sender, _decoyLifetime, position, rotation);
        }

        yield return null;
    }

    private float GetRndCoordBetween(float min, float max)
        => Random.Range(min, max);
}
