using System.Collections.Generic;
using UnityEngine;

public class PlayerModelVisuals : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Renderer _headMeshRenderer;
    [SerializeField] private Transform[] _jointsFixedHorizontallyToHead;

    [Header("First person")]
    [SerializeField] private string _firstPersonLayerName;
    [SerializeField] private Renderer[] _firstPersonObjects;

    private void Start()
    {
        HideHead();
        SetupFirstPersonObjects();
    }

    void LateUpdate()
    {
        if (!_player.HasAuthority())
            return;

        FixateJointsWithTheHead(_jointsFixedHorizontallyToHead);
    }

    private void SetupFirstPersonObjects()
    {
        foreach (var obj in _firstPersonObjects)
        {
            obj.gameObject.layer = LayerMask.NameToLayer(_firstPersonLayerName.Trim());
        }
    }

    private void HideHead()
        => _headMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

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
