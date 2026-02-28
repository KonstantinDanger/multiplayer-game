using System.Collections.Generic;
using UnityEngine;

public class AnimatorUpdater
{
    private readonly AnimatorUpdaterConfig _config;
    private readonly List<AnimatorUpdateData> _updateData = new();

    private GameObject _target;

    public AnimatorUpdater(AnimatorUpdaterConfig config)
        => _config = config;

    public void Initialize(GameObject targetUpdater, IEnumerable<Animator> animators)
    {
        _target = targetUpdater;

        foreach (Animator entity in animators)
        {
            if (entity == targetUpdater)
                continue;

            var anim = entity;
            anim.enabled = false;

            _updateData.Add(new() { Animator = anim, LastUpdateTime = Time.time + Random.Range(0, _config.MaxUpdateDelay) });
        }
    }

    public void Update(float deltaTime)
    {
        if (_target == null)
            return;

        foreach (AnimatorUpdateData animatorData in _updateData)
        {
            Animator animator = animatorData.Animator;

            float distanceToTarget = Vector3.Distance(animator.transform.position, _target.transform.position);

            float delay = CalculateUpdateDelay(distanceToTarget);

            if (delay <= 0)
            {
                animator.Update(deltaTime);

                UnityEngine.Debug.Log("update ");

                UnityEngine.Debug.Log("object: " + animatorData.Animator.transform.root.gameObject.name);
            }
            else if (Time.time - animatorData.LastUpdateTime >= delay)
            {
                UnityEngine.Debug.Log("calculate ");
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

        UnityEngine.Debug.Log("normalizedDistance " + normalizedDistance);

        return _config.DelayOverDistance.Evaluate(normalizedDistance) * _config.MaxUpdateDelay;
    }
}

