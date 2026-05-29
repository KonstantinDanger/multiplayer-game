using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorUpdater : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _owner;
    [SerializeField] private AnimatorUpdaterConfig _config;
    private readonly List<AnimatorUpdateData> _updateData = new();

    private bool _initialized;

    [TargetRpc]
    public void Initialize()
    {
        Animator[] animators = FindObjectsByType<Animator>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Animator entity in animators)
        {
            Animator anim = entity;

            if (anim == _animator)
                continue;

            anim.enabled = false;

            _updateData.Add(new()
            {
                Animator = anim,
                LastUpdateTime = Time.time + Random.Range(0, _config.MaxUpdateDelay)
            });
        }

        _initialized = true;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (!_initialized)
            return;

        if (_owner == null || _updateData.Count == 0)
            return;

        foreach (AnimatorUpdateData animatorData in _updateData)
        {
            Animator animator = animatorData.Animator;

            if (animator == null)
                continue;

            if (!animator.gameObject.activeInHierarchy)
                continue;

            float distanceToTarget = Vector3.Distance(animator.transform.position, _owner.transform.position);

            float delay = CalculateUpdateDelay(distanceToTarget);

            if (delay <= 0)
            {
                animator.Update(Time.deltaTime);

            }
            else if (Time.time - animatorData.LastUpdateTime >= delay)
            {
                animator.Update(Time.time - animatorData.LastUpdateTime);
                animatorData.LastUpdateTime = Time.time;
            }
        }
    }

    private float CalculateUpdateDelay(float distanceToTarget)
    {
        if (distanceToTarget <= _config.StartingUpdateDistance)
            return -1;

        float normalizedDistance = (distanceToTarget - _config.StartingUpdateDistance) / (_config.EndingUpdateDistance - _config.StartingUpdateDistance);

        return _config.DelayOverDistance.Evaluate(normalizedDistance) * _config.MaxUpdateDelay;
    }
}

