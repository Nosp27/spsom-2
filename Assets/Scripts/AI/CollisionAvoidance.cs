using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class CollisionAvoidance : MonoBehaviour
{
    [SerializeField] private float MaxDistance = 300;

    private Ship ship;
    private Bounds shipBounds;

    private float EmitterOffset;
    [SerializeField] private float EmitRadius;

    private RaycastHit[] hits;

    private LayerMask mask;

    private GameObject red;

    private Rigidbody rb;

    [SerializeField] private GameObject nearest;


    private void Awake()
    {
        mask = LayerMask.GetMask("Default");
    }

    // Start is called before the first frame update
    void Start()
    {
        hits = new RaycastHit[10];
        ship = GetComponentInParent<Ship>();
        rb = GetComponent<Rigidbody>();
        Collider[] allColliders = ship.GetComponentsInChildren<Collider>();
        allColliders = allColliders.Where(x => !x.isTrigger).ToArray();

        shipBounds = new Bounds();
        shipBounds.center = transform.position;
        foreach (Collider col in allColliders)
        {
            if (!col.isTrigger)
                shipBounds.Encapsulate(col.bounds);
        }

        EmitRadius = shipBounds.size.x;
        EmitterOffset = (shipBounds.extents.z + EmitRadius);
        
        red = GameObject.CreatePrimitive(PrimitiveType.Cube);
        red.SetActive(false);
        red.GetComponent<MeshRenderer>().material.color = Color.red;
        red.GetComponent<Collider>().enabled = false;
    }

    public Vector3 AvoidPoint(Vector3 targetPosition)
    {
        Vector3 distance = targetPosition - transform.position;
        Vector3 emitterDirection = distance.normalized;
        Vector3 emitterPosition = transform.position + emitterDirection * EmitRadius * 2;
        int nHits = Physics.SphereCastNonAlloc(
            emitterPosition, EmitRadius, emitterDirection, hits, distance.magnitude, mask
        );
        Debug.DrawRay(emitterPosition, distance, Color.cyan);

        print($"CA: {nHits} Hits");
        if (nHits == 0)
        {
            red.SetActive(false);
            return Vector3.zero;
        }

        float minHitDistance = hits[0].distance;
        RaycastHit nearestHit = hits[0];
        foreach (var hit in hits)
        {
            if (!CheckHit(hit))
                continue;
            if (minHitDistance > hit.distance)
            {
                minHitDistance = hit.distance;
                nearestHit = hit;
            }
        }
        
        if (!CheckHit(nearestHit))
        {
            red.SetActive(false);
            return Vector3.zero;
        }
        
        print(nearestHit.collider.gameObject.name);
        nearest = nearestHit.collider.gameObject;

        Bounds hitBounds = nearestHit.collider.bounds;
        float boundingSphereRadius = (hitBounds.center - hitBounds.min).magnitude;

        Vector3 avoidPoint;
        
        Vector3 rightAvoidPoint = transform.right * (shipBounds.extents.x + boundingSphereRadius + 5) + hitBounds.center;
        Vector3 leftAvoidPoint = -transform.right * (shipBounds.extents.x + boundingSphereRadius + 5) + hitBounds.center;

        float rightAngle = Vector3.Angle(transform.forward, rightAvoidPoint - transform.position);
        float leftAngle = Vector3.Angle(transform.forward, leftAvoidPoint - transform.position);

        if (leftAngle < rightAngle)
            avoidPoint = leftAvoidPoint;
        else
        {
            avoidPoint = rightAvoidPoint;
        }
        red.SetActive(true);
        red.transform.position = avoidPoint;
        return avoidPoint;
    }

    bool CheckHit(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            print("hit collider is null");
            return false;
        }
        if (hit.collider.isTrigger)
        {
            print("hit is trigger");
            return false;
        }
        if (hit.collider.attachedRigidbody == rb)
        {
            print("hit is owner");
            return false;
        }
        
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}