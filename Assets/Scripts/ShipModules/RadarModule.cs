using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RadarModule : MonoBehaviour
{
    [SerializeField] public float range;
    
    private RadarTarget thisShipTarget;
    public HashSet<RadarTarget> targetMarkers { get; private set; }
    public UnityEvent<RadarTarget> OnObjectEncounter { get; private set; }
    public UnityEvent<RadarTarget> OnObjectLost { get; private set; }

    private void Awake()
    {
        OnObjectEncounter = new UnityEvent<RadarTarget>();
        OnObjectLost = new UnityEvent<RadarTarget>();
    }

    void Start()
    {
        thisShipTarget = GetComponentInParent<RadarTarget>();
        targetMarkers = new HashSet<RadarTarget>();
    }

    public RadarTarget[] GetAllTargets()
    {
        return targetMarkers.ToArray();
    }

    void LateUpdate()
    {
        DetectTargets();
        List<RadarTarget> keysToDelete = new List<RadarTarget>();
        foreach (var target in targetMarkers)
        {
            if (!target || (target.transform.position - transform.position).magnitude > range)
                keysToDelete.Add(target);
        }

        foreach (var targetToDelete in keysToDelete)
        {
            RemoveTarget(targetToDelete);
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
                targetMarkers.Add(radarTarget);
                OnObjectEncounter.Invoke(radarTarget);
            }
        }
    }

    bool ShouldDetectTarget(RadarTarget radarTarget)
    {
        return radarTarget
               && radarTarget != thisShipTarget.GetComponent<RadarTarget>()
               && !targetMarkers.Contains(radarTarget);
    }

    void RemoveTarget(RadarTarget target)
    {
        if (!targetMarkers.Contains(target))
            return;

        OnObjectLost.Invoke(target);
        targetMarkers.Remove(target);
    }
}
