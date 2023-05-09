using System;
using GameControl.StateMachine;
using SpaceShip.PhysicalMovement;
using SpaceShip.ShipServices;
using UnityEngine;

public class PhysicalMovement : ShipMovementService
{
    public override Vector3 MoveAim { get; protected set; }
    public override float CurrentThrottle { get; protected set; }

    [SerializeField] private BaseEngineSplitter engineSplitter;
    [SerializeField] private float cruiseSpeed = 30f;
    [SerializeField] private bool rotateAtMoveAim = true;

    private Rigidbody m_Rigidbody;

    private Vector3 way;
    private Vector3 directWay;
    private Vector3 location;

    private Vector3 fullStopPoint;
    private Vector3 adjustVelocityPart;

    private bool forceBrake;
    private float m_ShipDrag;
    private Vector3 m_PinnedLookDirection;

    private Transform m_MovedTransform;

    public StateMachine stateMachine { get; private set; }

    void CalculateAttributes()
    {
        location = m_MovedTransform.position;
        Vector3 velocity = m_Rigidbody.velocity;

        way = MoveAim - m_MovedTransform.position;

        float directVelocityProjection = Utils.Projection(velocity, way);
        Vector3 directVelocityPart = way.normalized * directVelocityProjection;
        Vector3 sideVelocityPart = velocity - directVelocityPart;

        float part = (cruiseSpeed - directVelocityProjection) / cruiseSpeed;

        adjustVelocityPart = way.normalized * cruiseSpeed * part - 5 * sideVelocityPart;
        fullStopPoint = engineSplitter.PredictFinalPointNoDrag();
    }

    private void FlyToMoveAim()
    {
        engineSplitter.ApplyDeltaV(adjustVelocityPart);
    }

    private void Brake()
    {
        if (fullStopPoint == Vector3.zero)
        {
            EnableDrag();
        }
        else
        {
            engineSplitter.Brake();
        }
    }

    void EnableDrag()
    {
        m_Rigidbody.drag = m_ShipDrag;
    }

    void DisableDrag()
    {
        m_Rigidbody.drag = 0;
    }

    void PinLookVector()
    {
        m_PinnedLookDirection = way;
    }

    void ResetPinLookVector()
    {
        m_PinnedLookDirection = Vector3.zero;
    }

    void RotateAtTarget()
    {
        if (!rotateAtMoveAim)
            return;
        Vector3 targetDirection = m_PinnedLookDirection == Vector3.zero ? way : m_PinnedLookDirection;
        engineSplitter.ApplyRotationTorque(targetDirection);
    }

    StateMachine CreateStateMachine()
    {
        IState flyToDestination = LambdaState.New("Fly to destination")
            .WithEnterActions(DisableDrag, ResetPinLookVector)
            .WithTickActions(FlyToMoveAim, RotateAtTarget);
        IState brake = LambdaState.New("Brake")
            .WithTickActions(Brake);
        IState idle = LambdaState.New("Idle")
            .WithEnterActions(() => MoveAim = Vector3.zero, EnableDrag);
        // High velocity + Stop point is near the target or is jumped over the target ( |> ----- t --- x )
        Func<bool> needsBrakeCertainly = () =>
            forceBrake || m_Rigidbody.velocity.magnitude > 0.3f &&
            (Vector3.Distance(MoveAim, fullStopPoint) < 1.5f ||
             Vector3.Distance(location, fullStopPoint) > way.magnitude);

        // No need means !needForBrakesCertainly with extra conition: stop point is far from target
        Func<bool> noNeedForBrakes =
            () => Vector3.Distance(MoveAim, fullStopPoint) > 4f && !needsBrakeCertainly.Invoke();


        StateMachine sm = new StateMachine(AnyStatePriority.ANY_STATES_FIRST);
        // sm.AddAnyTransition(idle, () => MoveAim == Vector3.zero || At(MoveAim) && m_Rigidbody.velocity.magnitude < 0.1f);
        sm.AddTransition(idle, flyToDestination, () => MoveAim != Vector3.zero);
        sm.AddTransition(flyToDestination, brake, needsBrakeCertainly);
        sm.AddTransition(brake, idle,
            () => MoveAim == Vector3.zero || At(MoveAim));
        sm.AddTransition(brake, flyToDestination, noNeedForBrakes);

        return sm;
    }

    bool At(Vector3 point)
    {
        var x = Vector3.Distance(point, location) < 5f && m_Rigidbody.velocity.magnitude < 0.5f;
        return x;
    }

    ///// IShipMovementService implementation

    public override void Init(Transform t, MovementConfig config)
    {
        m_MovedTransform = t;
        m_Rigidbody = m_MovedTransform.GetComponent<Rigidbody>();
        m_Rigidbody.centerOfMass = Vector3.zero;
        m_ShipDrag = m_Rigidbody.drag;
        engineSplitter.Init(t, config);
        stateMachine = CreateStateMachine();
    }

    public override void Move(Vector3 v, float throttleCutoff)
    {
        // TODO: throttle cutoff
        forceBrake = false;
        v.y = location.y;
        MoveAim = v;
    }

    public override void TurnOnPlace(Vector3 v)
    {
        engineSplitter.ApplyRotationTorque(v - location);
    }

    public override void CancelMovement()
    {
        forceBrake = true;
    }

    public override void Tick()
    {
        Debug.DrawRay(location - Vector3.forward, way, Color.green);
        CalculateAttributes();
        stateMachine.Tick();
        engineSplitter.Tick();
    }

    public override bool IsMoving()
    {
        return MoveAim != Vector3.zero;
    }
}