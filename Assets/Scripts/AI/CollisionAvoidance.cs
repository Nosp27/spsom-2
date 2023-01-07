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

        red = GameObject.CreatePrimitive(PrimitiveType.Cube);
        red.SetActive(false);
        red.GetComponent<MeshRenderer>().material.color = Color.red;
        red.GetComponent<Collider>().enabled = false;
    }

    public Vector3 AvoidPoint(Vector3 targetPosition, AvoidDirection avoidDir, bool extendRange = true)
    {
        Vector3 distance = targetPosition - transform.position;
        Vector3 emitterDirection = distance.normalized;
        Vector3 emitterPosition = transform.position;
        float range = extendRange ? distance.magnitude * 2 : distance.magnitude;
        Debug.DrawRay(emitterPosition, emitterDirection * range, Color.blue);
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
        Debug.DrawRay(hitBounds.center, avoidLine* 100, Color.green);
        Debug.DrawRay(hitBounds.center, avoidLine* -100, Color.green);
        float avoidDirMultiplier = avoidDir == AvoidDirection.CW ? 1 : -1;
        Vector3 avoidPoint = avoidLine * avoidDirMultiplier * (shipBounds.extents.x + boundingSphereRadius + 5) +
                            hitBounds.center;
        return avoidPoint;
    }

    private Vector3 AvoidPointChained(Vector3 point, int limit, AvoidDirection avoidDir)
    {
        Debug.DrawLine(transform.position, point, Color.white);
        Vector3 finalAvoidPoint = Vector3.zero;
        Vector3 avoidPoint = point;
        int i = 0;
        for (i = 0; i < limit; i++)
        {
            avoidPoint = AvoidPoint(avoidPoint, avoidDir, i != 0);
            if (avoidPoint == Vector3.zero)
                break;
            finalAvoidPoint = avoidPoint;
        }

        return finalAvoidPoint;
    }

    public Vector3 AvoidPointCompound(Vector3 point, int limitOneSide = 10)
    {
        Vector3 avoidPoint = Vector3.zero;
        Vector3 distance = point - transform.position;

        Vector3 AP_CW = AvoidPointChained(point, limitOneSide, AvoidDirection.CW);
        Vector3 AP_CCW = AvoidPointChained(point, limitOneSide, AvoidDirection.CCW);

        if (AP_CW == Vector3.zero)
            avoidPoint = AP_CCW;

        if (AP_CCW == Vector3.zero)
            avoidPoint = AP_CW;

        float angleCW = Vector3.Angle(distance, AP_CW - transform.position);
        float angleCCW = Vector3.Angle(distance, AP_CCW - transform.position);

        if (AP_CW != Vector3.zero && AP_CCW != Vector3.zero)
        {
            if (angleCW < angleCCW)
                avoidPoint = AP_CW;
            else
                avoidPoint = AP_CCW;
        }

        red.SetActive(avoidPoint == Vector3.zero);
        red.transform.position = avoidPoint;
        return avoidPoint;
    }

    bool CheckHit(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            return false;
        }

        if (hit.collider.isTrigger)
        {
            return false;
        }

        if (hit.collider.attachedRigidbody == rb)
        {
            return false;
        }

        return true;
    }
}