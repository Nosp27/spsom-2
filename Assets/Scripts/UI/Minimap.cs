using System.Collections.Generic;
using System.Linq;
using GameEventSystem;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private RadarTarget m_PlayerRadarTarget;
    private GameObject m_PlayerRadarMarker;
    private RadarModule m_RadarModule;

    private Vector2 minimapCenter;
    private RectTransform minimapRect;

    private float scaleFactor;
    private Dictionary<RadarTarget, GameObject> targetMarkers;

    void Start()
    {
        EventLibrary.switchPlayerShip.AddListener(OnPlayerShipChanged);
        targetMarkers = new Dictionary<RadarTarget, GameObject>();
        minimapRect = GetComponent<RectTransform>();
    }

    void OnPlayerShipChanged(Ship old, Ship _new)
    {
        m_RadarModule = _new.GetComponentInChildren<RadarModule>();
        m_PlayerRadarTarget = _new.GetComponent<RadarTarget>();
        SyncMarkers();
    }
    
    void SyncMarkers()
    {
        foreach (RadarTarget targetMarker in targetMarkers.Keys.ToArray())
        {
            RemoveMarkerForTarget(targetMarker);
        }
        Destroy(m_PlayerRadarMarker);
        
        RadarTarget[] targets = m_RadarModule.GetAllTargets();
        foreach (RadarTarget target in targets)
        {
            SpawnMarkerForTarget(target);
        }
        m_PlayerRadarMarker = Instantiate(m_PlayerRadarTarget.RadarPrefabMinimap, minimapRect.transform);
    }

    void UpdateBoundaries()
    {
        scaleFactor = minimapRect.rect.size.x / 2 / m_RadarModule.range;
        minimapCenter = minimapRect.rect.center;
    }

    void SpawnMarkerForTarget(RadarTarget target)
    {
        if (target.RadarPrefabMinimap == null)
            return;
        
        GameObject minimapItem = Instantiate(target.RadarPrefabMinimap, minimapRect.transform);
        targetMarkers[target] = minimapItem;
    }

    void RemoveMarkerForTarget(RadarTarget target)
    {
        if (!targetMarkers.ContainsKey(target))
            return;
        
        GameObject marker = targetMarkers[target];
        targetMarkers.Remove(target);
        Destroy(marker);
    }

    void Update()
    {
        if (!m_RadarModule)
            return;

        UpdateBoundaries();
        foreach (KeyValuePair<RadarTarget,GameObject> targetMarker in targetMarkers)
        {
            PlaceMinimapItem(targetMarker.Value, targetMarker.Key);   
        }
        PlaceMinimapItem(m_PlayerRadarMarker, m_PlayerRadarTarget);
    }

    void PlaceMinimapItem(GameObject minimapItem, RadarTarget target)
    {
        Vector3 distanceFromAttachedShip = target.transform.position - m_PlayerRadarTarget.transform.position;
        Vector2 distance2D = new Vector2(distanceFromAttachedShip.x, distanceFromAttachedShip.z);
        Vector2 minimapPosition = minimapCenter + distance2D * scaleFactor;
        minimapItem.transform.localPosition = minimapPosition;
        
        Vector3 shipForwardTransformed = Vector3.ProjectOnPlane(target.transform.forward, Vector3.up);
        shipForwardTransformed = new Vector3(shipForwardTransformed.x, shipForwardTransformed.z, 0);
        minimapItem.transform.localRotation = Quaternion.LookRotation(Vector3.forward, shipForwardTransformed);
    }
}