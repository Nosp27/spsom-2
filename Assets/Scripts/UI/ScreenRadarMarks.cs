using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.LWRP;

public class ScreenRadarMarks : MonoBehaviour
{
    [SerializeField] private GameObject rangeCircle;
    [SerializeField] private float offset;

    private RadarModule m_RadarModule;
    public Dictionary<RadarTarget, GameObject> targetMarkers { get; private set; }

    void Start()
    {
        targetMarkers = new Dictionary<RadarTarget, GameObject>();
        rangeCircle.transform.localScale = 2 * offset * Vector3.one;
        GameController.Current.OnShipChange.AddListener(OnPlayerShipChanged);
    }

    void OnPlayerShipChanged(Ship old, Ship newShip)
    {
        if (m_RadarModule != null)
        {
            m_RadarModule.OnObjectEncounter.RemoveListener(SpawnMarker);
            m_RadarModule.OnObjectLost.RemoveListener(RemoveTargetMarker);
        }
        m_RadarModule = newShip.GetComponentInChildren<RadarModule>();
        SyncMarkers();
        m_RadarModule.OnObjectEncounter.AddListener(SpawnMarker);
        m_RadarModule.OnObjectLost.AddListener(RemoveTargetMarker);
    }

    void SyncMarkers()
    {
        foreach (var target in targetMarkers.Keys.ToArray())
        {
            RemoveTargetMarker(target);
        }
        
        RadarTarget[] targets = m_RadarModule.GetAllTargets();
        foreach (RadarTarget target in targets)
        {
            SpawnMarker(target);
        }
    }

    void SpawnMarker(RadarTarget newTarget)
    {
        if (newTarget.RadarPrefabWorld == null)
            return;
        var marker = Instantiate(newTarget.RadarPrefabWorld, transform);
        targetMarkers[newTarget] = marker;
    }

    void RemoveTargetMarker(RadarTarget lostTarget)
    {
        if (!targetMarkers.ContainsKey(lostTarget))
            return;
        Destroy(targetMarkers[lostTarget]);
        targetMarkers.Remove(lostTarget);
    }

    void LateUpdate()
    {
        if (m_RadarModule == null)
            return;
        
        foreach (KeyValuePair<RadarTarget,GameObject> targetMarker in targetMarkers)
        {
            PlaceMarker(targetMarker.Value.transform, targetMarker.Key.transform);
        }

        transform.position = m_RadarModule.transform.position;
    }

    void PlaceMarker(Transform marker, Transform target)
    {
        marker.position = transform.position + (target.position - transform.position).normalized *
            (offset + MarkerAdditionalOffset(marker));
        marker.LookAt(target);
    }

    float MarkerAdditionalOffset(Transform marker)
    {
        SpriteRenderer sr = marker.GetComponentInChildren<SpriteRenderer>();
        MeshRenderer mr = marker.GetComponentInChildren<MeshRenderer>();
        if (sr)
        {
            return sr.size.y + 1f;
        }

        if (mr)
        {
            return mr.bounds.size.y + 1f;
        }
        return 0f;
    }
}