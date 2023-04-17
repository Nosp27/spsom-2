using System;
using GameControl.StateMachine;
using SpaceShip.PhysicalMovement;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PhysicalEngineSplitter))]
[RequireComponent(typeof(Rigidbody))]
public class PhysicalMovement : MonoBehaviour
{
    public Vector3 MoveAim { get; private set; }

    [SerializeField] private Transform testTarget;
    [SerializeField] private Transform testIndicator;

    private Rigidbody m_Rigidbody;
    private PhysicalEngineSplitter m_Splitter;

    private Vector3 way;
    private Vector3 directWay;
    private Vector3 sideWay;

    private Vector3 fullStopPoint;
    private Vector3 directVelocityPart;
    private Vector3 sideVelocityPart;

    private bool forceBrake;
    private float m_ShipDrag;
    private Vector3 m_PinnedLookDirection;

    public StateMachine stateMachine { get; private set; }

    void CalculateAttributes()
    {
        Vector3 position = transform.position;
        Vector3 velocity = m_Rigidbody.velocity;

        way = MoveAim - position;
        directVelocityPart = way.normalized * Utils.Projection(velocity, way);
        sideVelocityPart = velocity - directVelocityPart;

        fullStopPoint = m_Splitter.PredictFinalPointNoDrag(m_Splitter.CalculateForce(-velocity, false));
        Vector3 finalPointWay = fullStopPoint - position;

        directWay = way.normalized * Utils.Projection(finalPointWay, way);
        sideWay = way - directWay;
    }

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_ShipDrag = m_Rigidbody.drag;
        m_Splitter = GetComponent<PhysicalEngineSplitter>();
        stateMachine = CreateStateMachine();
    }

    private void FlyToMoveAim()
    {
        m_Splitter.ApplyDeltaV(MoveAim - transform.position);
    }

    private void Brake()
    {
        if (fullStopPoint == Vector3.zero)
        {
            EnableDrag();
        }
        else
        {
            m_Splitter.ApplyDeltaV(-m_Rigidbody.velocity);
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
        m_Splitter.ApplyRotationTorque(targetDirection);
    }

    StateMachine CreateStateMachine()
    {
        IState adjustOrthogonal = LambdaState.New("Adjust orthogonal")
            .WithTickActions(() => m_Splitter.ApplyDeltaV(-sideVelocityPart), RotateAtTarget);
        IState flyToDestination = LambdaState.New("Fly to destination")
            .WithEnterActions(DisableDrag, ResetPinLookVector)
            .WithTickActions(FlyToMoveAim, RotateAtTarget)
            .WithExitActions(PinLookVector);
        IState brake = LambdaState.New("Brake")
            .WithTickActions(Brake, RotateAtTarget);
        IState angularBrake = LambdaState.New("Angular Brake")
            .WithTickActions(() => m_Splitter.AngularBrake(0.01f));
        IState idle = LambdaState.New("Idle")
            .WithEnterActions(() => MoveAim = Vector3.zero, EnableDrag);

        Func<bool> highSideDistance = () => sideWay.magnitude > 1f && sideVelocityPart.magnitude > 0.3f;
        
        // High velocity + Stop point is near the target or is jumped over the target ( |> ----- t --- f )
        Func<bool> needsBrakeCertainly = () =>
            forceBrake || m_Rigidbody.velocity.magnitude > 0.1f &&
            (Vector3.Distance(MoveAim, fullStopPoint) < 1.5f ||
             Vector3.Distance(transform.position, fullStopPoint) > way.magnitude);

        // No need means !needForBrakesCertainly with extra conition: stop point is far from target
        Func<bool> noNeedForBrakes =
            () => Vector3.Distance(MoveAim, fullStopPoint) > 4f && !needsBrakeCertainly.Invoke();


        StateMachine sm = new StateMachine(AnyStatePriority.ANY_STATES_FIRST);
        // sm.AddAnyTransition(idle, () => MoveAim == Vector3.zero || At(MoveAim) && m_Rigidbody.velocity.magnitude < 0.1f);
        sm.AddTransition(idle, flyToDestination, () => MoveAim != Vector3.zero);
        sm.AddTransition(flyToDestination, adjustOrthogonal, highSideDistance);
        sm.AddTransition(adjustOrthogonal, flyToDestination, () => !highSideDistance());
        sm.AddTransition(flyToDestination, brake, needsBrakeCertainly);
        sm.AddTransition(brake, angularBrake, () => MoveAim == Vector3.zero || At(MoveAim) && m_Rigidbody.velocity.magnitude < 0.1f);
        sm.AddTransition(angularBrake, idle, () => m_Rigidbody.angularVelocity.magnitude < 0.01f);
        sm.AddTransition(brake, flyToDestination, noNeedForBrakes);

        return sm;
    }

    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            forceBrake = !forceBrake;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            MoveAim = testTarget.position;
        }

        testIndicator.transform.position = fullStopPoint;
    }

    private void FixedUpdate()
    {
        CalculateAttributes();
        stateMachine.Tick();
    }

    bool At(Vector3 point)
    {
        var x = Vector3.Distance(point, transform.position) < 2 && m_Rigidbody.velocity.magnitude < 0.5f;
        return x;
    }
}