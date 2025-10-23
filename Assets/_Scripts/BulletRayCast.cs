using Mirror;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BulletRayCast : NetworkBehaviour
{
    [Header("Settings")]
    public float lineDuration = 0.05f;
    public Color lineColor = Color.yellow;
    public float lineWidth = 0.05f;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.enabled = false;
    }

    public void StartBulletView(Vector3 start, Vector3 end)
        => CmdStartBulletView(start, end);

    [Command]
    private void CmdStartBulletView(Vector3 start, Vector3 end)
        => RpcStartBulletView(start, end);

    [ClientRpc]
    private void RpcStartBulletView(Vector3 start, Vector3 end)
        => StartCoroutine(FireRay(start, end));

    private IEnumerator FireRay(Vector3 origin, Vector3 endPos)
    {
        // Draw the line
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPos);
        lineRenderer.enabled = true;

        // Wait briefly then hide line
        yield return new WaitForSeconds(lineDuration);
        lineRenderer.enabled = false;
    }
}
