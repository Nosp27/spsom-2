using SpaceShip.ShipServices;
using UnityEngine;

public class MovementService
{
    public Vector3 MoveAim { get; private set; }
    public float CurrentThrottle { get; private set; }

    private MovementConfig m_Config;

    private EngineBalancer m_EngineBalancer;
    private Vector3 m_TurnTarget;
    private float m_ThrottleCutoff = 1;
    private Transform m_MovedTransform;

    private Rigidbody rb;

    public MovementService(Transform t, MovementConfig config)
    {
        m_Config = config;

        m_EngineBalancer = t.GetComponent<EngineBalancer>();
        rb = t.GetComponent<Rigidbody>();
        m_MovedTransform = t;
    }

    public void Move(Vector3 target, float throttleCutoff)
    {
        m_ThrottleCutoff = throttleCutoff;
        MoveAim = new Vector3(target.x, m_MovedTransform.position.y, target.z);
    }

    private void TurnAt()
    {
        Vector3 point = MoveAim == Vector3.zero ? m_TurnTarget : MoveAim;
        Quaternion currentRotation = rb.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(point - rb.position, Vector3.up);
        float angle = Quaternion.Angle(currentRotation, targetRotation);

        if (m_EngineBalancer != null)
            m_EngineBalancer.BalanceEnginePower(CurrentThrottle, point, angle);

        if (angle > 30)
        {
            float rollAngle = angle > 60 ? 20 : 12;
            float left = Utils.Projection(point - rb.position, m_MovedTransform.right) > 0 ? -1 : 1;
            targetRotation = Quaternion.AngleAxis(rollAngle * left, m_MovedTransform.forward) * targetRotation;
        }

        Quaternion rotStep =
            Quaternion.SlerpUnclamped(currentRotation, targetRotation, Time.deltaTime * m_Config.RotationSpeed);
        rb.rotation = rotStep;
    }

    public void TurnOnPlace(Vector3 target)
    {
        Vector3 distance = target - m_MovedTransform.position;
        if (distance.magnitude < 1)
            return;

        if (MoveAim.magnitude > 0)
        {
            m_TurnTarget = Vector3.zero;
            return;
        }

        m_TurnTarget = target;
        TurnAt();
    }

    public void CancelMovement()
    {
        MoveAim = Vector3.zero;
        CurrentThrottle = 0;
    }

    void MoveStep()
    {
        Vector3 point = MoveAim;
        var position = m_MovedTransform.position;

        if (point.magnitude == 0)
        {
            CurrentThrottle = 0;
            return;
        }


        if ((point - position).magnitude < 9f)
        {
            MoveAim = Vector3.zero;
            CurrentThrottle = 0;
            m_TurnTarget = position + m_MovedTransform.forward * 10;
            m_TurnTarget.y = position.y;
            return;
        }

        // With angle correction throttle adds up
        // currentThrottle =
        //     Mathf.Pow(Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(movedTransform.forward, point - movedTransform.position)), 2);

        var forward = m_MovedTransform.forward;
        CurrentThrottle =
            1f / (1f + Mathf.Tan(Mathf.Deg2Rad * Vector3.Angle(forward, point - position)));

        CurrentThrottle = Mathf.Clamp(CurrentThrottle, 0, m_ThrottleCutoff);

        TurnAt();
        Vector3 moveVector = forward;
        moveVector.y = position.y;
        // movedTransform.position += moveVector * Time.deltaTime * linearSpeed * currentThrottle;
        if (rb.velocity.magnitude < m_Config.LinearSpeed)
            rb.AddForce(moveVector * Time.deltaTime * m_Config.Power * CurrentThrottle *
                        Mathf.Clamp01((point - position).magnitude / 80f));
    }

    public void Tick()
    {
        if (MoveAim != Vector3.zero)
        {
            MoveStep();
        }
        else if (m_TurnTarget != Vector3.zero)
        {
            TurnAt();
        }
    }

    public bool IsMoving()
    {
        return MoveAim != Vector3.zero;
    }
}