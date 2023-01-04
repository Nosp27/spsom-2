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

    public Vector3 AvoidPoint(Vector3 targetPosition, AvoidDirection avoidDir)
    {
        Vector3 distance = targetPosition - transform.position;
        Vector3 emitterDirection = distance.normalized;
        Vector3 emitterPosition = transform.position + emitterDirection * EmitRadius * 2;
        int nHits = Physics.SphereCastNonAlloc(
            emitterPosition, EmitRadius, emitterDirection, hits, distance.magnitude, mask
        );

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

        nearest = nearestHit.collider.gameObject;

        Bounds hitBounds = nearestHit.collider.bounds;
        float boundingSphereRadius = (hitBounds.center - hitBounds.min).magnitude;

        Vector3 avoidPoint;

        if (avoidDir == AvoidDirection.CW)
        {
            Vector3 rightAvoidPoint =
                transform.right * (shipBounds.extents.x + boundingSphereRadius + 5) + hitBounds.center;
            avoidPoint = rightAvoidPoint;
        }
        else
        {
            Vector3 leftAvoidPoint =
                -transform.right * (shipBounds.extents.x + boundingSphereRadius + 5) + hitBounds.center;
            avoidPoint = leftAvoidPoint;
        }

        return avoidPoint;
    }

    private Vector3 AvoidPointChained(Vector3 point, int limit, AvoidDirection avoidDir)
    {
        Vector3 finalAvoidPoint = Vector3.zero;
        Vector3 avoidPoint;
        for (int i = 0; i < limit; i++)
        {
            avoidPoint = AvoidPoint(point, avoidDir);
            if (avoidPoint == Vector3.zero)
                break;
            finalAvoidPoint = avoidPoint;
        }

        return finalAvoidPoint;
    }

    public Vector3 AvoidPointCompound(Vector3 point, int limitOneSide = 10)
    {
        Vector3 avoidPoint = Vector3.zero;

        Vector3 AP_CW = AvoidPointChained(point, limitOneSide, AvoidDirection.CW);
        Vector3 AP_CCW = AvoidPointChained(point, limitOneSide, AvoidDirection.CCW);

        if (AP_CW == Vector3.zero)
            avoidPoint = AP_CCW;

        if (AP_CCW == Vector3.zero)
            avoidPoint = AP_CW;

        float angleCW = Vector3.Angle(transform.forward, AP_CW - transform.position);
        float angleCCW = Vector3.Angle(transform.forward, AP_CCW - transform.position);

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