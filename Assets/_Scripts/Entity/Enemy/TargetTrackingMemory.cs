using Mirror;
using System.Collections;
using UnityEngine;

public class TargetTrackingMemory : NetworkBehaviour, ITargetTrackingMemory
{
    public NetworkBehaviour Target { get; private set; }

    private TargetTrackingConfig _config;
    private IEnumerator _forgetTargetRoutine;

    public void Initialize(TargetTrackingConfig config)
    {
        _config = config;
        _forgetTargetRoutine = ForgetTargetRoutine();
    }

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
