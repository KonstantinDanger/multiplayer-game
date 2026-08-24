using UnityEngine;

public class SurfaceChecker : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _ceilingCheck;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private Transform _ledgeCheck;
    [SerializeField] private BoxCollider _aboveLedgeCheck;

    [Header("Values")]
    [SerializeField] private float _slopeCheckDistance = 1f;
    [SerializeField] private float _groundCheckRadius = 0.4f;
    [SerializeField] private float _ceilingCheckRadius = 0.4f;
    [SerializeField] private float _wallCheckRadius = 0.4f;
    [SerializeField] private float _ledgeCheckRadius = 0.4f;
    [SerializeField] private float _ledgeAboveCheckDistance = 1f;

    [Header("Layers")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _ceilingLayer;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _ledgeLayer;

    [Header("Debug")]
    [SerializeField] private bool _debugEnabled = true;

    public bool IsGrounded => Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundLayer, QueryTriggerInteraction.Ignore);
    public bool IsUnderCeiling => Physics.CheckSphere(_ceilingCheck.position, _ceilingCheckRadius, _ceilingLayer, QueryTriggerInteraction.Ignore);
    public bool LedgeDetected => Physics.CheckSphere(_ledgeCheck.position, _ledgeCheckRadius, _ledgeLayer, QueryTriggerInteraction.Ignore);
    public bool IsLedgeAboveEmpty => !Physics.CheckBox(_aboveLedgeCheck.bounds.center, _aboveLedgeCheck.size / 2, transform.rotation, _ledgeLayer, QueryTriggerInteraction.Ignore);

    public Vector3 AboveLedgeCheckPosition => _aboveLedgeCheck.bounds.center + _aboveLedgeCheck.transform.forward * _ledgeAboveCheckDistance;

    public Transform GroundCheck => _groundCheck;

    public float DistanceToGround
    {
        get
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit))
                return hit.distance;

            return 0;
        }
    }

    public bool IsOnSlope
    {
        get
        {
            bool onSlope = false;

            if (Physics.Raycast(_groundCheck.position, -_groundCheck.up, out RaycastHit hit, _slopeCheckDistance))
                onSlope = Mathf.RoundToInt(Vector3.Angle(hit.normal, transform.up)) != 0;

            return onSlope;
        }
    }

    private void FixedUpdate()
    {

    }

    public bool TryGetCurrentWallInfo(Vector3 checkDirection, out Vector3 surfaceNormal)
    {
        Ray ray = new(_wallCheck.position, checkDirection);

        bool isTouchingWall = Physics.SphereCast(_wallCheck.position, _wallCheckRadius, checkDirection, out RaycastHit hit, _wallCheckRadius, _wallLayer, QueryTriggerInteraction.Ignore);
        surfaceNormal = hit.normal;

        return isTouchingWall;
    }

    private void OnDrawGizmos()
    {
        if (!_debugEnabled)
            return;

        Color color = Color.green;
        color.a = 0.5f;
        Gizmos.color = color;

        if (_groundCheck)
            Gizmos.DrawSphere(_groundCheck.position, _groundCheckRadius);

        if (_ceilingCheck)
            Gizmos.DrawSphere(_ceilingCheck.position, _ceilingCheckRadius);

        if (_ledgeCheck)
            Gizmos.DrawSphere(_ledgeCheck.position, _ledgeCheckRadius);

        //Gizmos.color = !IsLedgeAboveEmpty ? Color.red : Color.green;
        //Gizmos.DrawWireCube(AboveLedgeCheckPosition, _aboveLedgeCheck.bounds.size);
    }
}