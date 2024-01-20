using System;
using Logic;
using SpaceShip.PhysicalMovement;
using SpaceShip.ShipServices;
using UnityEngine;

public class PhysicalMovement : ShipMovementService
{
    public override Vector3 MoveAim { get; protected set; }
    public override float CurrentThrottle { get; protected set; }

    [SerializeField] private BaseEngineSplitter engineSplitter;
    [SerializeField] private float cruiseSpeed = 30f;
    [SerializeField] private bool forceBrakeOnly;
    public float CruiseSpeed => cruiseSpeed;

    private HEADING_MODE headingMode;
    private float slowModeThreshold = 10;

    private float m_ThrottleCutoff;
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

        if (MoveAim == default)
        {
            way = default;
        }
        else
        {
            way = MoveAim - m_MovedTransform.position;   
        }

        float directVelocityProjection = Utils.Projection(velocity, way);
        Vector3 directVelocityPart = way.normalized * directVelocityProjection;
        Vector3 sideVelocityPart = velocity - directVelocityPart;

        float part = (cruiseSpeed - directVelocityProjection) / cruiseSpeed;

        adjustVelocityPart = way.normalized * cruiseSpeed * part - 5 * sideVelocityPart;
        fullStopPoint = engineSplitter.PredictFinalPointNoDrag();
    }

    public void ChangeCruiseSpeed(int newCruiseSpeed)
    {
        cruiseSpeed = newCruiseSpeed;
    }

    private void ActionFlyToMoveAim()
    {
        engineSplitter.ApplyDeltaV(adjustVelocityPart, m_ThrottleCutoff);
    }

    private void ActionBrake()
    {
        if (fullStopPoint == Vector3.zero)
        {
            ActionEnableDrag();
        }
        else
        {
            engineSplitter.Brake();
        }
    }

    private void OnDisable()
    {
        ActionEnableDrag();
    }

    void ActionEnableDrag()
    {
        m_Rigidbody.drag = m_ShipDrag;
    }

    void ActionDisableDrag()
    {
        m_Rigidbody.drag = 0;
    }

    void ActionResetPinLookVector()
    {
        m_PinnedLookDirection = Vector3.zero;
    }

    void ActionRotateAtTarget()
    {
        if (headingMode != HEADING_MODE.LOCKED_HEADING)
            return;
        Vector3 targetDirection = m_PinnedLookDirection == Vector3.zero ? way : m_PinnedLookDirection;
        engineSplitter.ApplyRotationTorque(targetDirection);
    }

    StateMachine CreateStateMachine()
    {
        IState flyToDestination = LambdaState.New("Fly to destination")
            .WithEnterActions(ActionDisableDrag, ActionResetPinLookVector)
            .WithTickActions(ActionFlyToMoveAim, ActionRotateAtTarget);
        IState brake = LambdaState.New("Brake")
            .WithTickActions(ActionBrake);
        IState idle = LambdaState.New("Idle")
            .WithEnterActions(() => MoveAim = Vector3.zero, ActionEnableDrag);

        // High velocity + Stop point is near the target or is jumped over the target ( |> ----- t --- x )
        Func<bool> needsBrakeCertainly = () =>
            forceBrake || !forceBrakeOnly && m_Rigidbody.velocity.magnitude > 0.3f &&
            (Vector3.Distance(MoveAim, fullStopPoint) < 1.5f ||
             Vector3.Distance(location, fullStopPoint) > way.magnitude);

        // No need means !needForBrakesCertainly with extra conition: stop point is far from target
        Func<bool> noNeedForBrakes =
            () => Vector3.Distance(MoveAim, fullStopPoint) > 4f && !needsBrakeCertainly.Invoke();

        Func<bool> moveCancelledWithoutBraking = () => forceBrakeOnly && (MoveAim == default || At(MoveAim));

        StateMachine sm = new StateMachine(AnyStatePriority.ANY_STATES_FIRST);
        // sm.AddAnyTransition(idle, () => MoveAim == Vector3.zero || At(MoveAim) && m_Rigidbody.velocity.magnitude < 0.1f);
        sm.AddTransition(idle, flyToDestination, () => MoveAim != Vector3.zero);
        sm.AddTransition(flyToDestination, brake, needsBrakeCertainly);
        sm.AddTransition(flyToDestination, idle, moveCancelledWithoutBraking);
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

    public override void Move(Vector3 v)
    {
        forceBrake = false;
        v.y = location.y;
        MoveAim = v;
    }

    public override void MoveAtDirection(Vector3 target)
    {
        Vector3 position = m_MovedTransform.position;
        target = position + (target - position).normalized * 300;
        Move(target);
    }

    public override void LimitThrottle(float limit)
    {
        m_ThrottleCutoff = limit;
    }

    public override void ChangeHeadingMode(HEADING_MODE headingMode)
    {
        this.headingMode = headingMode;
    }

    public override void TurnAt(Vector3 v)
    {
        engineSplitter.ApplyRotationTorque(v - location);
    }

    public override void CancelMovement(bool forceBrake=true)
    {
        MoveAim = Vector3.zero;
        if (forceBrake)
            this.forceBrake = true;
    }

    private void Tick()
    {
        Debug.DrawRay(location - Vector3.forward, way, Color.green);
        CalculateAttributes();
        stateMachine.Tick();
        engineSplitter.Tick();
    }

    private void FixedUpdate()
    {
        Tick();
    }

    public override bool IsMoving()
    {
        return MoveAim != Vector3.zero;
    }
}