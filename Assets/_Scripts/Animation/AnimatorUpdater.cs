using System.Collections.Generic;
using UnityEngine;

public class AnimatorUpdater
{
    private readonly AnimatorUpdaterConfig _config;
    private readonly List<AnimatorUpdateData> _updateData = new();

    private GameObject _target;

    public AnimatorUpdater(AnimatorUpdaterConfig config)
        => _config = config;

    public void Initialize(GameObject targetUpdater)
    {
        Animator[] animators = Object.FindObjectsByType<Animator>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Animator senderAnimator = targetUpdater.GetComponentInChildren<Animator>();

        UnityEngine.Debug.Log("targetUpdater " + targetUpdater);
        UnityEngine.Debug.Log("animators.length " + animators.Length);

        _target = targetUpdater;

        foreach (Animator entity in animators)
        {
            Animator anim = entity;

            if (anim == senderAnimator)
                continue;

            anim.enabled = false;

            _updateData.Add(new()
            {
                Animator = anim,
                LastUpdateTime = Time.time + Random.Range(0, _config.MaxUpdateDelay)
            });
        }
    }

    public void Update(float deltaTime)
    {
        if (_target == null || _updateData.Count == 0)
            return;

        foreach (AnimatorUpdateData animatorData in _updateData)
        {
            Animator animator = animatorData.Animator;

            if (animator == null)
                continue;

            float distanceToTarget = Vector3.Distance(animator.transform.position, _target.transform.position);

            float delay = CalculateUpdateDelay(distanceToTarget);

            if (delay <= 0)
            {
                animator.Update(deltaTime);

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

