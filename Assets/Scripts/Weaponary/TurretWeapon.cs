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

    private void Start()
    {
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
        }
    }

    public override void Track(Transform target)
    {
        m_TrackTarget = target;
        m_AimTarget = Vector3.zero;

        if (target)
        {
            targetRb = target.GetComponent<Rigidbody>();
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
            
            if (leadFire && rb != null && targetRb != null)
            {
                targetPoint =
                    InterceptionCalculator.ShootingDirection(
                        rb,
                        targetRb,
                        m_Gun.BulletSpeed
                    );
            }
        } else if (m_AimTarget != Vector3.zero)
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