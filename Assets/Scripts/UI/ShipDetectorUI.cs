using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDetectorUI : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private GameObject rangeCircle;
    [SerializeField] private float offset;
    

    private Ship thisShip;
    private float markerAdditionalOffset;
    private Dictionary<GameObject, GameObject> targetMarkers;

    void Start()
    {
        markerAdditionalOffset = markerPrefab.GetComponentInChildren<SpriteRenderer>().size.y + 1f;
        thisShip = GetComponentInParent<Ship>();
        targetMarkers = new Dictionary<GameObject, GameObject>();
        rangeCircle.transform.localScale = 2 * offset * Vector3.one;

        if (markerPrefab == null)
        {
            markerPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            markerPrefab.GetComponent<Collider>().enabled = false;
            markerPrefab.GetComponent<MeshRenderer>().material.color = Color.green;
            markerPrefab.transform.position = new Vector3(100, 1000, 100);
        }
    }

    void LateUpdate()
    {
        DetectTargets();
        List<GameObject> keysToDelete = new List<GameObject>();
        foreach (var target in targetMarkers.Keys)
        {
            GameObject marker = targetMarkers[target];
            if ((target.transform.position - transform.position).magnitude > range)
                keysToDelete.Add(target);
            else
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
            Ship shipComponent = col.GetComponentInParent<Ship>();
            if (
                shipComponent
                && shipComponent != thisShip
                && !targetMarkers.ContainsKey(shipComponent.gameObject)
            )
            {
                GameObject marker = Instantiate(markerPrefab, transform);
                PlaceMarker(marker.transform, shipComponent.transform);
                targetMarkers.Add(shipComponent.gameObject, marker);
            }
        }
    }

    void RemoveTarget(GameObject marker, GameObject target)
    {
        if (!targetMarkers.ContainsKey(target))
            return;

        targetMarkers.Remove(target);
        Destroy(marker);
    }

    void PlaceMarker(Transform marker, Transform target)
    {
        marker.position = transform.position + (target.position - transform.position).normalized * (offset + markerAdditionalOffset);
        marker.LookAt(target);
    }
}