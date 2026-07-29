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

    private bool _isPlaying = false;

    private IAbilityUser AbilityUser => _abilityUser.Value;

    private void OnEnable()
    {
        AbilityUser.OnStartUsing += HandleUsageStart;
        AbilityUser.OnPerform += HandlePerform;
    }

    private void OnDisable()
        => AbilityUser.OnStartUsing -= HandleUsageStart;

    private void HandleUsageStart(UseAbilityData data)
        => RpcPlayAttackAnimation(data);

    private void HandlePerform(float duration)
    {

    }

    [Command(requiresAuthority = false)]
    private void CmdPlayAttackAnimation(UseAbilityData data)
        => RpcPlayAttackAnimation(data);

    [ClientRpc]
    private void RpcPlayAttackAnimation(UseAbilityData data)
        => StartCoroutine(StartAnimationRoutine(data));

    private IEnumerator StartAnimationRoutine(UseAbilityData data)
    {
        if (_isPlaying)
            yield break;

        _isPlaying = true;

        float animatorSpeed = _animator.speed;

        float desiredSpeed = data.UsagePreparationTime == 0 ? 0 :
            animatorSpeed * data.UsagePreparationAnimDuration / data.UsagePreparationTime;

        _animator.speed = desiredSpeed;

        Play(data.PreparationAnimationName);

        yield return new WaitForSeconds(data.UsagePreparationTime);

        _animator.speed = animatorSpeed;

        Play(data.UsageAnimationName);

        _isPlaying = false;
    }

    private void Play(string animationName)
        => _animator.CrossFadeInFixedTime(animationName, _animationTransitionTime, _abilityAnimationsLayer);
}
