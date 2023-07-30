using System;
using System.Linq;
using UnityEngine;


public enum AvoidDirection
{
    CW,
    CCW,
}


[RequireComponent(typeof(Rigidbody))]
public class CollisionAvoidance : MonoBehaviour
{
    private Ship ship;
    private Bounds shipBounds;

    [SerializeField] private float EmitRadius;
    [SerializeField] private float parktronicRadius = 10;
    [SerializeField] private float parktronicAvoidMagnitude = 10;

    [SerializeField] private float minObstacleDistanceForAvoid = 70;

    private RaycastHit[] hits;
    private Collider[] parktronicCols;

    private LayerMask mask;

    private GameObject red;

    private Rigidbody rb;

    [SerializeField] private bool debug;

    [SerializeField] private GameObject nearest;

    // Cache direction for current final position to avoid re-desiding which side to go (CW or CCW, based on min angle)
    private Tuple<Vector3, bool> cachedDirectionForPosition;

    private void Awake()
    {
        mask = LayerMask.GetMask("Default");
    }

    // Start is called before the first frame update
    void Start()
    {
        hits = new RaycastHit[50];
        parktronicCols = new Collider[50];
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

        red = GameObject.CreatePrimitive(PrimitiveType.Cube);
        red.SetActive(false);
        red.GetComponent<MeshRenderer>().material.color = Color.red;
        red.GetComponent<Collider>().enabled = false;
    }

    public Vector3 AvoidPoint(Vector3 targetPosition, AvoidDirection avoidDir, bool extendRange = true)
    {
        Vector3 distance = targetPosition - transform.position;
        Vector3 emitterDirection = distance.normalized;
        Vector3 emitterPosition = transform.position + distance.normalized * (EmitRadius * 2);
        float range = extendRange ? distance.magnitude * 2 : distance.magnitude;
        if (debug)
        {
            Debug.DrawRay(emitterPosition, emitterDirection.normalized * range, Color.white);
        }

        int nHits = Physics.SphereCastNonAlloc(
            emitterPosition, EmitRadius, emitterDirection, hits, range, mask
        );

        if (nHits == 0)
        {
            red.SetActive(false);
            return Vector3.zero;
        }

        float minHitDistance = 0;
        RaycastHit nearestHit = hits[0];
        bool hitDetected = false;

        for (int i = 0; i < nHits; i++)
        {
            var hit = hits[i];
            if (!CheckHit(hit))
                continue;
            if (!hitDetected || (minHitDistance > hit.distance))
            {
                hitDetected = true;
                minHitDistance = hit.distance;
                nearestHit = hit;
            }
        }

        if (!hitDetected)
        {
            red.SetActive(false);
            return Vector3.zero;
        }

        nearest = nearestHit.collider.gameObject;

        Bounds hitBounds = nearestHit.collider.bounds;
        float boundingSphereRadius = (hitBounds.center - hitBounds.min).magnitude;

        Vector3 avoidLine = Vector3.Cross(distance, Vector3.up).normalized;
        float avoidDirMultiplier = avoidDir == AvoidDirection.CW ? 1 : -1;
        Vector3 avoidPoint = avoidLine * avoidDirMultiplier * (shipBounds.extents.x + boundingSphereRadius + 5) +
                             hitBounds.center;

        if (debug)
        {
            Debug.DrawLine(avoidPoint, nearestHit.point, Color.yellow);
        }

        return avoidPoint;
    }

    private Vector3 AvoidPointChained(Vector3 point, int limit, AvoidDirection avoidDir)
    {
        if (debug)
        {
            Debug.DrawLine(transform.position, point, Color.white);
        }

        Vector3 finalAvoidPoint = Vector3.zero;
        Vector3 avoidPoint = point;
        for (int i = 0; i < limit; i++)
        {
            var old = avoidPoint;
            avoidPoint = AvoidPoint(avoidPoint, avoidDir, i != 0);

            if (avoidPoint == Vector3.zero)
                break;

            if (debug)
            {
                Debug.DrawLine(old, avoidPoint, Color.red);
            }

            finalAvoidPoint = avoidPoint;
        }

        if (debug)
        {
            Debug.DrawLine(transform.position, finalAvoidPoint, Color.red);
        }

        return finalAvoidPoint;
    }

    public Vector3 AvoidPointCompound(Vector3 point, int limitOneSide = 10)
    {
        Vector3 parktronicPoint = ApplyParktronic();
        if (parktronicPoint != Vector3.zero && (parktronicPoint - transform.position).magnitude > 3f)
        {
            return parktronicPoint;
        }

        Vector3 avoidPoint = Vector3.zero;
        Vector3 distance = point - transform.position;

        Vector3 AP_CW = AvoidPointChained(point, limitOneSide, AvoidDirection.CW);
        Vector3 AP_CCW = AvoidPointChained(point, limitOneSide, AvoidDirection.CCW);

        if (AP_CW == Vector3.zero || AP_CCW == Vector3.zero)
        {
            avoidPoint = Vector3.zero;
        }
        else
        {
            if (cachedDirectionForPosition == null || cachedDirectionForPosition.Item1 != point)
            {
                float angleCW = Vector3.Angle(distance, AP_CW - transform.position);
                float angleCCW = Vector3.Angle(distance, AP_CCW - transform.position);

                cachedDirectionForPosition = new Tuple<Vector3, bool>(point, angleCW < angleCCW);
            }
            
            if (cachedDirectionForPosition.Item2)
                avoidPoint = AP_CW;
            else
                avoidPoint = AP_CCW;
        }

        red.SetActive(avoidPoint == Vector3.zero);
        red.transform.position = avoidPoint;
        return avoidPoint;
    }

    private Vector3 ApplyParktronic()
    {
        if (parktronicRadius == 0)
            return Vector3.zero;

        int nHits = Physics.OverlapSphereNonAlloc(transform.position, parktronicRadius, parktronicCols, mask);

        Vector3 avoidDir = Vector3.zero;
        for (int i = 0; i < nHits; ++i)
        {
            if (CheckCollider(parktronicCols[i]))
            {
                avoidDir += transform.position - parktronicCols[i].transform.position;
            }
        }

        return transform.position + avoidDir.normalized * parktronicAvoidMagnitude;
    }

    bool CheckHit(RaycastHit hit)
    {
        if (Vector3.Distance(transform.position, hit.point) > minObstacleDistanceForAvoid)
        {
            return false;
        }

        return CheckCollider(hit.collider);
    }

    bool CheckCollider(Collider col)
    {
        if (col == null)
        {
            return false;
        }

        if (col.isTrigger)
        {
            return false;
        }

        if (col.attachedRigidbody == rb)
        {
            return false;
        }

        return true;
    }
}