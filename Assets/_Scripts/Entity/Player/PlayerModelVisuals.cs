using System.Collections.Generic;
using UnityEngine;

public class PlayerModelVisuals : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Renderer _headMeshRenderer;
    [SerializeField] private Transform[] _jointsFixedHorizontallyToHead;

    void LateUpdate()
    {
        if (!_player.HasAuthority())
            return;

        _headMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

        //_headMeshRenderer.enabled = false;

        FixateJointsWithTheHead(_jointsFixedHorizontallyToHead);
    }

    private void FixateJointsWithTheHead(IEnumerable<Transform> joints)
    {
        //Vector3 forwardDirection = Vector3.ProjectOnPlane(head.forward, Vector3.up);

        foreach (var joint in joints)
        {
            Vector3 euler = joint.localEulerAngles;
            euler.z = 0f;
            joint.localEulerAngles = euler;
        }
    }
}
