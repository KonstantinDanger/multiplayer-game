using AYellowpaper;
using Mirror;
using UnityEngine;

public class AttackAnimation : NetworkBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private InterfaceReference<IAttacker> _attacker;
    [SerializeField] private string _attackAnimationName = "Attack";
    [SerializeField] private float _animationTransitionTime = 0.15f;
    [SerializeField] private int _layer;

    private IAttacker Attacker => _attacker.Value;

    private void OnEnable()
        => Attacker.OnAttack += HandleAttack;

    private void OnDisable()
        => Attacker.OnAttack -= HandleAttack;

    private void HandleAttack()
        => CmdPlayAttackAnimation();

    [Command(requiresAuthority = false)]
    private void CmdPlayAttackAnimation()
        => RpcPlayAttackAnimation();

    [ClientRpc]
    private void RpcPlayAttackAnimation()
        => _anim.CrossFadeInFixedTime(_attackAnimationName, _animationTransitionTime, _layer);

}
