using UnityEngine;

public class FlightController : MonoBehaviour, IFlightController
{
    [SerializeField] private float flightHeight = 3f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float heightAdjustmentSpeed = 2f;
    [SerializeField] private float bobAmplitude = 0.3f;
    [SerializeField] private float bobFrequency = 1f;
    [SerializeField] private LayerMask _groundLayer;

    private float bobTimer;

    private void Update()
        => bobTimer += Time.deltaTime * bobFrequency;

    public void FlyTowards(Vector3 targetPosition, float customHeight = -1f)
    {
        float desiredHeight = customHeight > 0 ? customHeight : flightHeight;

        // Target position with height
        Vector3 targetWithHeight = new Vector3(
            targetPosition.x,
            targetPosition.y + desiredHeight,
            targetPosition.z
        );

        // Add bobbing effect
        float bobOffset = Mathf.Sin(bobTimer) * bobAmplitude;
        targetWithHeight.y += bobOffset;

        // Move towards target
        Vector3 direction = (targetWithHeight - transform.position).normalized;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWithHeight,
            moveSpeed * Time.deltaTime
        );

        // Smooth rotation
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    public void MaintainFlightHeight()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, _groundLayer))
        {
            float targetY = hit.point.y + flightHeight;
            float currentY = transform.position.y;

            transform.position = new Vector3(
                transform.position.x,
                Mathf.Lerp(currentY, targetY, heightAdjustmentSpeed * Time.deltaTime),
                transform.position.z
            );
        }
    }
}
