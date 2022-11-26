using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDetectorUI : MonoBehaviour
{
    [SerializeField] public float range;
    [SerializeField] private GameObject rangeCircle;
    [SerializeField] private float offset;


    private RadarTarget thisShipTarget;
    public Dictionary<RadarTarget, GameObject> targetMarkers { get; private set; }

    void Start()
    {
        thisShipTarget = GetComponentInParent<RadarTarget>();
        targetMarkers = new Dictionary<RadarTarget, GameObject>();
        rangeCircle.transform.localScale = 2 * offset * Vector3.one;
    }

    void LateUpdate()
    {
        DetectTargets();
        List<RadarTarget> keysToDelete = new List<RadarTarget>();
        foreach (var target in targetMarkers.Keys)
        {
            GameObject marker = targetMarkers[target];
            if (!target || (target.transform.position - transform.position).magnitude > range)
                keysToDelete.Add(target);
            else if (ShouldTrackTarget(target))
                PlaceMarker(marker.transform, target.transform);
        }

        foreach (var targetToDelete in keysToDelete)
        {
            RemoveTarget(targetMarkers[targetToDelete], targetToDelete);
        }
    }


    void DetectTargets()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, range);
        foreach (var col in cols)
        {
            RadarTarget radarTarget = col.GetComponentInParent<RadarTarget>();
            if (ShouldDetectTarget(radarTarget))
            {
                GameObject marker = null;
                if (ShouldTrackTarget(radarTarget))
                {
                    marker = Instantiate(radarTarget.RadarPrefabWorld, transform);
                    PlaceMarker(marker.transform, radarTarget.transform);
                }

                targetMarkers.Add(radarTarget, marker);
            }
        }
    }

    bool ShouldDetectTarget(RadarTarget radarTarget)
    {
        return radarTarget
               && radarTarget != thisShipTarget.GetComponent<RadarTarget>()
               && !targetMarkers.ContainsKey(radarTarget);
    }

    bool ShouldTrackTarget(RadarTarget target)
    {
        return target.RadarPrefabWorld != null;
    }

    void RemoveTarget(GameObject marker, RadarTarget target)
    {
        if (!targetMarkers.ContainsKey(target))
            return;

        targetMarkers.Remove(target);
        Destroy(marker);
    }

    void PlaceMarker(Transform marker, Transform target)
    {
        marker.position = transform.position + (target.position - transform.position).normalized *
            (offset + MarkerAdditionalOffset(marker));
        marker.LookAt(target);
    }

    float MarkerAdditionalOffset(Transform marker)
    {
        return marker.GetComponentInChildren<SpriteRenderer>().size.y + 1f;
    }
}