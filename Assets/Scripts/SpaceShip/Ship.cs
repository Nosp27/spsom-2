using System;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public bool HasGunnery;
    public float LinearSpeed = 3f;
    public float RotationSpeed = 3f;
    public float Power = 30f;
    private EngineBalancer engineBalancer;
    private Camera currentCamera;
    public Vector3 MoveAim { get; private set; }
    public float currentThrottle { get; private set; }
    public bool Alive;
    public List<Gun> Weapons;
    private List<Turret> Turrets;
    private Vector3 TurnTarget;
    public bool isPlayerShip;
    [SerializeField] private float softerMultiplier = 1;
    private Rigidbody rb;
    private float lastAngle;


    // Start is called before the first frame update
    void Start()
    {
        InitWeaponary();
        isPlayerShip = gameObject.CompareTag("PlayerShip");
        HasGunnery = Weapons.Count > 0;
        engineBalancer = GetComponent<EngineBalancer>();
        Alive = true;
        rb = GetComponent<Rigidbody>();
    }

    public void InitWeaponary()
    {
        Weapons = new List<Gun>(GetComponentsInChildren<Gun>());
        Turrets = new List<Turret>(GetComponentsInChildren<Turret>());
    }

    void Die()
    {
        Alive = false;
    }

    private void FixedUpdate()
    {
        if (Alive)
        {
            if (MoveAim != Vector3.zero)
            {
                MoveStep();
            }
            else if (TurnTarget != Vector3.zero)
            {
                TurnAt();
            }
        }
    }

    public void TurnOnPlace(Vector3 target)
    {
        if (!Alive)
            return;

        Vector3 distance = target - transform.position;
        if (distance.magnitude < 1)
            return;

        if (MoveAim.magnitude > 0)
        {
            TurnTarget = Vector3.zero;
            return;
        }

        TurnTarget = target;
        TurnAt();
    }

    public void Move(Vector3 target)
    {
        if (!Alive)
            return;

        MoveAim = new Vector3(target.x, transform.position.y, target.z);
    }

    public bool IsMoving()
    {
        return MoveAim != Vector3.zero;
    }

    public bool IsDifferentMovePoint(Vector3 otherMovePoint)
    {
        // If cancelling movement, do not consider it as different move point
        if (otherMovePoint == Vector3.zero)
            return false;
        
        // If current move point is zero, any other is different
        if (MoveAim == Vector3.zero)
        {
            return true;
        }

        // If points are near, say they are not different
        if ((MoveAim - otherMovePoint).magnitude < 1f)
        {
            return false;
        }

        return true;
    }

    public void CancelMovement()
    {
        MoveAim = Vector3.zero;
        currentThrottle = 0;
    }

    public void Shoot(Vector3 cursor)
    {
        if (!Alive)
            return;

        foreach (var w in Weapons)
        {
            w.Shoot();
        }
    }

    public void Aim(Vector3 cursor)
    {
        if (!Alive)
            return;

        foreach (var t in Turrets)
        {
            t.Aim(cursor);
        }
    }

    public bool Aimed(Vector3 cursor)
    {
        if (!Alive)
            return false;

        foreach (var t in Turrets)
        {
            if (t.Aimed(cursor))
                return true;
        }

        return false;
    }


    void MoveStep()
    {
        Vector3 point = MoveAim;
        if (point.magnitude == 0)
        {
            currentThrottle = 0;
            return;
        }


        if ((point - transform.position).magnitude < 9f)
        {
            MoveAim = Vector3.zero;
            currentThrottle = 0;
            TurnTarget = transform.position + transform.forward * 10;
            TurnTarget.y = transform.position.y;
            return;
        }

        // With angle correction throttle adds up
        // currentThrottle =
        //     Mathf.Pow(Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(transform.forward, point - transform.position)), 2);

        currentThrottle =
            1f / (1f + Mathf.Tan(Mathf.Deg2Rad * Vector3.Angle(transform.forward, point - transform.position)));

        currentThrottle = Mathf.Clamp01(currentThrottle);
        TurnAt(false);
        Vector3 moveVector = transform.forward;
        moveVector.y = transform.position.y;
        // transform.position += moveVector * Time.deltaTime * LinearSpeed * currentThrottle;
        if (rb.velocity.magnitude < LinearSpeed)
            rb.AddForce(moveVector * Time.deltaTime * Power * currentThrottle *
                        Mathf.Clamp01((point - transform.position).magnitude / 80f));
    }

    public void TurnAt(bool lerp = true)
    {
        Vector3 point = MoveAim == Vector3.zero ? TurnTarget : MoveAim;
        Quaternion currentRotation = rb.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(point - rb.position, Vector3.up);
        float angle = Quaternion.Angle(currentRotation, targetRotation);
        
        if (engineBalancer != null)
            engineBalancer.BalanceEnginePower(currentThrottle, point, angle);

        if (angle > 30)
        {
            float rollAngle = angle > 60 ? 20 : 12;
            float left = LinUtils.Projection(point - rb.position, transform.right) > 0 ? -1 : 1;
            targetRotation = Quaternion.AngleAxis(rollAngle * left, transform.forward) * targetRotation;
        }

        float angleEffect = GetRotationSmoothMultiplier(angle);
        if (angle > 10f || lastAngle == 0 || true)
        {
            Quaternion rotStep =
                Quaternion.SlerpUnclamped(currentRotation, targetRotation, Time.deltaTime * RotationSpeed);
            lastAngle = Quaternion.Angle(currentRotation, rotStep);
            rb.rotation = rotStep;
        }
        else
        {
            rb.rotation = Quaternion.RotateTowards(currentRotation, targetRotation, lastAngle);
        }
    }

    private float GetRotationSmoothMultiplier(float angle)
    {
        float from = 0;
        float to = 20;
        if (angle <= from || angle >= to)
            return 1;
        float span = to - from;
        float x = angle;
        float smoothFuncValue =
            softerMultiplier * (-Mathf.Sin((x / (span / 2f) - 1 - from) * Mathf.PI / 2f) + 1) / 2f + 1;
        return smoothFuncValue;
    }
}