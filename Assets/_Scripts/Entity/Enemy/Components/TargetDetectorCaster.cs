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

    public override IEnumerable<GameObject> DetectAllTargets(GameObject sender, Vector3 origin, Vector3 direction, float detectionRadius, bool includeSender = false)
        => DoCast(sender, new Ray(origin, direction), detectionRadius, includeSender);

    public override GameObject DetectNearestTarget(GameObject sender, Vector3 origin, Vector3 direction, float detectionRadius, bool includeSender = false)
        => DetectAllTargets(sender, origin, direction, detectionRadius, includeSender).FirstOrDefault();

    private IEnumerable<GameObject> DoCast(GameObject sender, Ray ray, float range, bool includeSender)
    {
        ray.direction = CalculateSpreadDirection(ray.direction);

        RaycastHit[] results = new RaycastHit[MaxTargetHits];
        int targetResults;

        targetResults = _useSphereCast
            ? Physics.SphereCastNonAlloc(ray, _sphereRange, results, range, layersToDetect, QueryTriggerInteraction.Ignore)
            : Physics.RaycastNonAlloc(ray, results, range, layersToDetect, QueryTriggerInteraction.Ignore);

        if (!includeSender && sender == results.FirstOrDefault().collider.gameObject)
        {
            Array.Copy(results, 0, results, 0, targetResults);
            targetResults--;
        }

        if (targetResults > 1)
            Array.Sort(results, 0, targetResults, Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));

        return results.Select(hit => hit.collider.gameObject);
    }

    private Vector3 CalculateSpreadDirection(Vector3 direction)
    {
        float horizontalAngle = Random.Range(-_horizontalSpreadAngle, _horizontalSpreadAngle);
        float verticalAngle = Random.Range(-_verticalSpreadAngle, _verticalSpreadAngle);

        Quaternion rotation = Quaternion.Euler(horizontalAngle, verticalAngle, 0f);

        return rotation * direction;
    }
}
