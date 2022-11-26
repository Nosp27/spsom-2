using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private RadarTarget PlayerRadarTarget => GameController.Current.PlayerShip.GetComponent<RadarTarget>();
    private ShipDetectorUI detector;

    private Vector2 minimapCenter;
    private RectTransform minimapRect;

    private float scaleFactor;
    private Dictionary<RadarTarget, GameObject> targetMarkers;

    void Start()
    {
        targetMarkers = new Dictionary<RadarTarget, GameObject>();
        detector = PlayerRadarTarget.GetComponentInChildren<ShipDetectorUI>();
        minimapRect = GetComponent<RectTransform>();
        MaybeRegisterMinimapItem(PlayerRadarTarget);
    }

    void UpdateBoundaries()
    {
        scaleFactor = minimapRect.rect.size.x / 2 / detector.range;
        minimapCenter = minimapRect.rect.center;
    }

    void Update()
    {
        if (!PlayerRadarTarget)
            return;

        if (detector?.targetMarkers == null || detector.targetMarkers.Count == 0)
            return;
        
        UpdateBoundaries();
        
        foreach (var target in detector.targetMarkers.Keys)
        {
            MaybeRegisterMinimapItem(target);
        }

        HashSet<RadarTarget> keysToDelete = new HashSet<RadarTarget>();
        foreach (var item in targetMarkers)
        {
            RadarTarget target = item.Key;
            GameObject marker = item.Value;
            if (!target || (target.transform.position - PlayerRadarTarget.transform.position).magnitude > detector.range)
                keysToDelete.Add(target);
            else
                PlaceMinimapItem(marker, target);
        }

        foreach (var target in keysToDelete)
        {
            GameObject marker = targetMarkers[target];
            targetMarkers.Remove(target);
            Destroy(marker);
        }
        PlaceMinimapItem(targetMarkers[PlayerRadarTarget], PlayerRadarTarget);
        
    }

    void MaybeRegisterMinimapItem(RadarTarget target)
    {
        if (targetMarkers.ContainsKey(target))
            return;

        GameObject minimapItem = Instantiate(target.RadarPrefabMinimap, minimapRect.transform);
        targetMarkers[target] = minimapItem;
    }

    void PlaceMinimapItem(GameObject minimapItem, RadarTarget target)
    {
        Vector3 distanceFromAttachedShip = target.transform.position - PlayerRadarTarget.transform.position;
        Vector2 distance2D = new Vector2(distanceFromAttachedShip.x, distanceFromAttachedShip.z);
        Vector2 minimapPosition = minimapCenter + distance2D * scaleFactor;
        minimapItem.transform.localPosition = minimapPosition;
        
        Vector3 shipForwardTransformed = Vector3.ProjectOnPlane(target.transform.forward, Vector3.up);
        shipForwardTransformed = new Vector3(shipForwardTransformed.x, shipForwardTransformed.z, 0);
        minimapItem.transform.rotation = Quaternion.LookRotation(Vector3.forward, shipForwardTransformed);
    }
}