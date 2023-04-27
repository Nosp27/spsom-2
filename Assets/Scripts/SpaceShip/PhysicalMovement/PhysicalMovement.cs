using System;
using GameControl.StateMachine;
using SpaceShip.PhysicalMovement;
using SpaceShip.ShipServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicalMovement : ShipMovementService
{
    public override Vector3 MoveAim { get; protected set; }
    public override float CurrentThrottle { get; protected set; }

    [SerializeField] private Physical4EngineSplitter engineSplitter;

    private Rigidbody m_Rigidbody;

    private Vector3 way;
    private Vector3 directWay;
    private Vector3 sideWay;

    private Vector3 fullStopPoint;
    private Vector3 directVelocityPart;
    private Vector3 sideVelocityPart;

    private bool forceBrake;
    private float m_ShipDrag;
    private Vector3 m_PinnedLookDirection;

    private Transform m_MovedTransform;

    public StateMachine stateMachine { get; private set; }

    void CalculateAttributes()
    {
        Vector3 position = m_MovedTransform.position;
        Vector3 velocity = m_Rigidbody.velocity;

        way = MoveAim - position;
        directVelocityPart = way.normalized * Utils.Projection(velocity, way);
        sideVelocityPart = velocity - directVelocityPart;

        fullStopPoint = engineSplitter.PredictFinalPointNoDrag();
        Vector3 finalPointWay = fullStopPoint - position;

        directWay = way.normalized * Utils.Projection(finalPointWay, way);
        sideWay = way - directWay;
    }

    private void FlyToMoveAim()
    {
        engineSplitter.ApplyDeltaV(MoveAim - m_MovedTransform.position);
    }

    private void Brake()
    {
        if (fullStopPoint == Vector3.zero)
        {
            EnableDrag();
        }
        else
        {
            engineSplitter.ApplyDeltaV(-m_Rigidbody.velocity);
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
        Vector3 targetDirection = m_PinnedLookDirection == Vector3.zero ? way : m_PinnedLookDirection;
        engineSplitter.ApplyRotationTorque(targetDirection);
    }

    StateMachine CreateStateMachine()
    {
        IState adjustOrthogonal = LambdaState.New("Adjust orthogonal")
            .WithTickActions(() => engineSplitter.ApplyDeltaV(-sideVelocityPart), RotateAtTarget);
        IState flyToDestination = LambdaState.New("Fly to destination")
            .WithEnterActions(DisableDrag, ResetPinLookVector)
            .WithTickActions(FlyToMoveAim, RotateAtTarget)
            .WithExitActions(PinLookVector);
        IState brake = LambdaState.New("Brake")
            .WithTickActions(Brake, RotateAtTarget);
        IState angularBrake = LambdaState.New("Angular Brake")
            .WithTickActions(() => engineSplitter.AngularBrake(0.01f));
        IState idle = LambdaState.New("Idle")
            .WithEnterActions(() => MoveAim = Vector3.zero, EnableDrag);

        Func<bool> highSideDistance = () => sideWay.magnitude > 1f && sideVelocityPart.magnitude > 0.3f;

        // High velocity + Stop point is near the target or is jumped over the target ( |> ----- t --- f )
        Func<bool> needsBrakeCertainly = () =>
            forceBrake || m_Rigidbody.velocity.magnitude > 0.1f &&
            (Vector3.Distance(MoveAim, fullStopPoint) < 1.5f ||
             Vector3.Distance(m_MovedTransform.position, fullStopPoint) > way.magnitude);

        // No need means !needForBrakesCertainly with extra conition: stop point is far from target
        Func<bool> noNeedForBrakes =
            () => Vector3.Distance(MoveAim, fullStopPoint) > 4f && !needsBrakeCertainly.Invoke();


        StateMachine sm = new StateMachine(AnyStatePriority.ANY_STATES_FIRST);
        // sm.AddAnyTransition(idle, () => MoveAim == Vector3.zero || At(MoveAim) && m_Rigidbody.velocity.magnitude < 0.1f);
        sm.AddTransition(idle, flyToDestination, () => MoveAim != Vector3.zero);
        sm.AddTransition(flyToDestination, adjustOrthogonal, highSideDistance);
        sm.AddTransition(adjustOrthogonal, flyToDestination, () => !highSideDistance());
        sm.AddTransition(flyToDestination, brake, needsBrakeCertainly);
        sm.AddTransition(brake, angularBrake,
            () => MoveAim == Vector3.zero || At(MoveAim) && m_Rigidbody.velocity.magnitude < 0.1f);
        sm.AddTransition(angularBrake, idle, () => m_Rigidbody.angularVelocity.magnitude < 0.01f);
        sm.AddTransition(brake, flyToDestination, noNeedForBrakes);

        return sm;
    }

    bool At(Vector3 point)
    {
        var x = Vector3.Distance(point, m_MovedTransform.position) < 2 && m_Rigidbody.velocity.magnitude < 0.5f;
        return x;
    }

    ///// IShipMovementService implementation

    public override void Init(Transform t, MovementConfig config)
    {
        m_MovedTransform = t;
        m_Rigidbody = m_MovedTransform.GetComponent<Rigidbody>();
        m_ShipDrag = m_Rigidbody.drag;
        engineSplitter.Init(t, config);
        stateMachine = CreateStateMachine();
    }

    public override void Move(Vector3 v, float throttleCutoff)
    {
        // TODO: throttle cutoff
        forceBrake = false;
        MoveAim = v;
    }

    public override void TurnOnPlace(Vector3 v)
    {
    }

    public override void CancelMovement()
    {
        forceBrake = true;
    }

    public override void Tick()
    {
        CalculateAttributes();
        stateMachine.Tick();
        engineSplitter.Tick();
    }

    public override bool IsMoving()
    {
        return MoveAim != Vector3.zero;
    }
    
    // Debug
    [SerializeField] private Transform testTarget;
    [SerializeField] private MovementConfig testConfig;
    private void Start()
    {
        Init(gameObject.transform, testConfig);
    }
    
    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Move(testTarget.position, 1);
        }
    }

    private void FixedUpdate()
    {
        Tick();
    }
    // End Debug
}