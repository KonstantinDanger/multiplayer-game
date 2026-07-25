using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class TargetDetectorCaster : TargetDetector
{
    private const int MaxTargetHits = 64;

    [SerializeField, Range(0, 360)] private float _verticalSpreadAngle;
    [SerializeField, Range(0, 360)] private float _horizontalSpreadAngle;

    [Header("Penetration")]
    [SerializeField, Range(1, 20)] private int _penetrationCount;
    //[SerializeField, Range(0, 100)] private float _damageDecayPercentOverPenetration;

    [Header("Sphere cast")]
    [SerializeField] private bool _useSphereCast;
    [SerializeField, Range(0f, 5f)] private float _sphereRange;

    public override int DetectAllTargets(GameObject sender, Vector3 origin, Vector3 direction, float detectionRadius, out List<GameObject> targets, bool includeSender = false)
    {
        Ray ray = new Ray(origin, direction);

        ray.direction = CalculateSpreadDirection(ray.direction);

        RaycastHit[] results = new RaycastHit[MaxTargetHits];
        int resultCount;

        resultCount = _useSphereCast
            ? Physics.SphereCastNonAlloc(ray, _sphereRange, results, detectionRadius, layersToDetect, QueryTriggerInteraction.Ignore)
            : Physics.RaycastNonAlloc(ray, results, detectionRadius, layersToDetect, QueryTriggerInteraction.Ignore);

        if (!includeSender && sender == results.FirstOrDefault().collider.gameObject)
        {
            Array.Copy(results, 0, results, 0, resultCount);
            resultCount--;
        }

        targets = new(results.Select(hit => hit.collider.gameObject));

        return resultCount;
    }

    private Vector3 CalculateSpreadDirection(Vector3 direction)
    {
        float horizontalAngle = Random.Range(-_horizontalSpreadAngle, _horizontalSpreadAngle);
        float verticalAngle = Random.Range(-_verticalSpreadAngle, _verticalSpreadAngle);

        Quaternion rotation = Quaternion.Euler(horizontalAngle, verticalAngle, 0f);

        return rotation * direction;
    }
}
