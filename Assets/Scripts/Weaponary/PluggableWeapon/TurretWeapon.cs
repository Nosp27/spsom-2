using System;
using UnityEngine;

public class TurretWeapon : Weapon
{
    [SerializeField] private bool leadFire;
    private Turret m_Turret;
    private Gun m_Gun;
    private Rigidbody rb;
    private Rigidbody targetRb;

    private Transform m_TrackTarget;
    private Vector3 m_AimTarget;
    private bool m_Aimed;

    [SerializeField] private bool debug;
    private GameObject red;

    public override float maxCooldown
    {
        get => m_Gun.maxCooldown;
        protected set => throw new NotImplementedException();
    }
    
    public override float cooldown
    {
        get => m_Gun.cooldown;
        protected set => throw new NotImplementedException();
    }

    private void Start()
{
    if (debug)
    {
        red = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var mat = red.GetComponent<MeshRenderer>().material;
        mat.color = Color.red;
        red.GetComponent<MeshRenderer>().material = mat;
        red.GetComponent<Collider>().enabled = false;
        red.name = "debugRed";
    }
    rb = GetComponentInParent<Rigidbody>();
    m_Turret = GetComponentInChildren<Turret>();
    m_Gun = GetComponentInChildren<Gun>();
}

private void LateUpdate()
{
    m_Aimed = false;
    Vector3 aimTargetPoint = GetAimTargetPoint();
    if (aimTargetPoint != Vector3.zero)
    {
        m_Turret.Aim(aimTargetPoint);
        m_Aimed = m_Turret.Aimed(aimTargetPoint);
        if (debug)
        {
            var currentAimColor = m_Aimed ? Color.green : Color.red;
            var distance = Vector3.Distance(m_Turret.AimingRay.transform.position, aimTargetPoint);
            Debug.DrawLine(m_Turret.AimingRay.transform.position, aimTargetPoint, Color.green);
            Debug.DrawRay(m_Turret.AimingRay.transform.position, m_Turret.AimingRay.transform.forward * distance, currentAimColor);
        }
    }
}

public override void Track(Transform target)
{
    m_TrackTarget = target;
    m_AimTarget = Vector3.zero;

    if (target)
    {
        targetRb = target.GetComponent<Rigidbody>();
        if (debug)
        {
            print($"Target rb: {targetRb}");
        }
    }
}

public override void Aim(Vector3 target)
{
    m_AimTarget = target;
    m_TrackTarget = null;
    if (target != Vector3.zero)
        m_Turret.Aim(GetAimTargetPoint());
}

public override bool Aimed()
{
    return m_Aimed;
}

public override void Fire()
{
    m_Gun.Shoot();
}

Vector3 GetAimTargetPoint()
{
    Vector3 targetPoint;
    if (m_TrackTarget)
    {
        targetPoint = m_TrackTarget.position;

        if (leadFire && targetRb != null)
        {
            if (rb == null)
            {
                targetPoint =
                    InterceptionCalculator.ShootingDirection(
                        transform,
                        targetRb,
                        m_Gun.BulletSpeed
                    );    
            }
            else
            {
                targetPoint =
                    InterceptionCalculator.ShootingDirection(
                        rb,
                        targetRb,
                        m_Gun.BulletSpeed
                    );
            }
            
            if (debug)
            {
                print("Got target point");
                red.transform.position = targetPoint;
            }
        }
    }
    else if (m_AimTarget != Vector3.zero)
    {
        targetPoint = m_AimTarget;
    }
    else
    {
        targetPoint = Vector3.zero;
    }

    return targetPoint;
}
}