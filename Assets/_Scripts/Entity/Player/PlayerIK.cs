using UnityEngine;

public class PlayerIK : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private Transform _headObject;

    [SerializeField] private bool _enabled = true;

    private void LateUpdate()
    {
        if (!_player.HasAuthority())
            return;

        if (!_enabled)
            return;

        _headObject.transform.position = _player.Camera.Transform.position;
    }
}
