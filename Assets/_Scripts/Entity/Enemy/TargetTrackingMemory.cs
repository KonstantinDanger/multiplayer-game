using Mirror;
using System.Collections;
using UnityEngine;

public class TargetTrackingMemory : NetworkBehaviour, ITargetTrackingMemory
{
    [SerializeField] private TargetTrackingConfig _config;
    public NetworkBehaviour Target { get; private set; }

    public bool IsTracking => Target != null;

    private IEnumerator _forgetTargetRoutine;

    private void Start()
        => _forgetTargetRoutine = ForgetTargetRoutine();

    public void Memorize(NetworkBehaviour target)
    {
        if (Target == target)
            return;

        if (!_config.CanRetargetWhileTargeted && Target != null)
            return;

        Target = target;

        StopCoroutine(_forgetTargetRoutine);
    }

    public void Forget()
        => StartCoroutine(_forgetTargetRoutine);

    private IEnumerator ForgetTargetRoutine()
    {
        yield return new WaitForSeconds(_config.ForgettingTimeout);

        Target = null;
    }
}
