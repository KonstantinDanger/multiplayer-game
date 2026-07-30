using AYellowpaper;
using Mirror;
using System.Collections;
using UnityEngine;

public class AbilityVisualPerformer : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private InterfaceReference<IAbilityUser> _abilityUser;
    [SerializeField] private float _animationTransitionTime = 0.15f;
    [SerializeField] private int _abilityAnimationsLayer = 1;

    private Coroutine _animationRoutine;
    private float _initialAnimatorSpeed;

    private IAbilityUser AbilityUser => _abilityUser.Value;

    private void Start()
        => _initialAnimatorSpeed = _animator.speed;

    private void OnEnable()
    {
        AbilityUser.OnPreparation += HandleUsagePreparation;
        AbilityUser.OnPerform += HandlePerform;
        AbilityUser.OnFinish += HandleFinish;
    }

    private void OnDisable()
    {
        AbilityUser.OnPreparation -= HandleUsagePreparation;
        AbilityUser.OnPerform -= HandlePerform;
        AbilityUser.OnFinish -= HandleFinish;
    }

    private void HandleUsagePreparation(IAbilityPresentationData data, float duration)
        => PlayAnimation(data.PreparationAnimation, duration);
    //RpcPlayAttackAnimation(data, duration);

    private void HandlePerform(IAbilityPresentationData data, float duration)
        => PlayAnimation(data.UsageAnimation, duration);

    private void HandleFinish(IAbilityPresentationData data) { }

    private void PlayAnimation(AnimationClip animation, float playTime)
    {
        if (animation == null)
            return;

        if (_animationRoutine != null)
            StopCoroutine(_animationRoutine);

        _animationRoutine = StartCoroutine(StartAnimationRoutine(animation, playTime));
    }

    private IEnumerator StartAnimationRoutine(AnimationClip animation, float playTime)
    {
        float animatorSpeed = _initialAnimatorSpeed;

        float animationDuration = animation.length;
        float desiredSpeed = playTime == 0f ? 0f : animatorSpeed * animationDuration / playTime;

        _animator.speed = desiredSpeed;

        Play(animation.name);

        yield return new WaitForSeconds(animationDuration);

        _animationRoutine = null;
    }

    private void Play(string animationName)
        => _animator.CrossFadeInFixedTime(animationName, _animationTransitionTime, _abilityAnimationsLayer);

    //[Command(requiresAuthority = false)]
    //private void CmdPlayAttackAnimation(UseAbilityData data)
    //    => RpcPlayAttackAnimation(data);

    //[ClientRpc]
    //private void RpcPlayAttackAnimation(UseAbilityData data)
    //    => StartCoroutine(StartAnimationRoutine(data));

    //private IEnumerator StartAnimationRoutine(UseAbilityData data)
    //{
    //    if (_isPlaying)
    //        yield break;

    //    _isPlaying = true;

    //    float animatorSpeed = _animator.speed;

    //    float desiredSpeed = data.UsagePreparationTime == 0 ? 0 :
    //        animatorSpeed * data.UsagePreparationAnimDuration / data.UsagePreparationTime;

    //    _animator.speed = desiredSpeed;

    //    Play(data.PreparationAnimationName);

    //    yield return new WaitForSeconds(data.UsagePreparationTime);

    //    _animator.speed = _initialAnimatorSpeed;

    //    Play(data.UsageAnimationName);

    //    _isPlaying = false;
    //}
}
