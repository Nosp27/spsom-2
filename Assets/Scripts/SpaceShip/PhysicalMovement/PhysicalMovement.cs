using System;
using GameControl.StateMachine;
using SpaceShip.PhysicalMovement;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private Vector3 finalPoint;
    private Vector3 directVelocityPart;
    private Vector3 sideVelocityPart;

    private StateMachine sm;
    
    void CalculateAttributes()
    {
        Vector3 position = transform.position;
        Vector3 velocity = m_Rigidbody.velocity;
        
        way = MoveAim - position;
        directVelocityPart = way.normalized * Utils.Projection(velocity, way);
        sideVelocityPart = velocity - directVelocityPart;

        finalPoint = PredictFinalPoint(m_Splitter.CalculateForce(-velocity));
        Vector3 finalPointWay = finalPoint - position;

        directWay = way.normalized * Utils.Projection(finalPointWay, way);
        sideWay = way - directWay;
    }

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Splitter = GetComponent<PhysicalEngineSplitter>();
        sm = CreateStateMachine();
    }

    private void FlyToMoveAim()
    {
        m_Splitter.ApplyDeltaV(MoveAim - transform.position);
        m_Splitter.ApplyRotationTorque(way);
    }

    private void Brake()
    {
        m_Splitter.ApplyDeltaV(-m_Rigidbody.velocity);
    }

    StateMachine CreateStateMachine()
    {
        IState adjustOrthogonal = LambdaState.create.WithTickActions(() => m_Splitter.ApplyDeltaV(-sideVelocityPart));
        IState flyToDestination = LambdaState.create.WithTickActions(FlyToMoveAim);
        IState brake = LambdaState.create.WithTickActions(Brake);
        IState idle = LambdaState.create.WithEnterActions(() => MoveAim = Vector3.zero);

        Func<bool> highSideDistance = () => sideWay.magnitude > 1f && sideVelocityPart.magnitude > 0.3f;
        Func<bool> needsBrake = () =>
            m_Rigidbody.velocity.magnitude > 0.1f &&
            (Vector3.Distance(MoveAim, finalPoint) < 2f || Vector3.Distance(MoveAim, finalPoint) > way.magnitude);

        StateMachine sm = new StateMachine(AnyStatePriority.ANY_STATES_FIRST);
        sm.AddAnyTransition(idle, () => MoveAim == Vector3.zero || At(MoveAim) && m_Rigidbody.velocity.magnitude < 0.1f);
        sm.AddTransition(idle, flyToDestination, () => MoveAim != Vector3.zero);
        sm.AddTransition(flyToDestination, adjustOrthogonal, highSideDistance);
        sm.AddTransition( adjustOrthogonal, flyToDestination, () => !highSideDistance());
        sm.AddTransition(flyToDestination, brake, needsBrake);
        sm.AddTransition( brake, flyToDestination, () => !needsBrake());

        return sm;
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            MoveAim = testTarget.position;
        }

        testIndicator.transform.position = finalPoint;
    }

    private void FixedUpdate()
    {
        CalculateAttributes();
        sm.Tick();
    }

    bool At(Vector3 point)
    {
        var x = Vector3.Distance(point, transform.position) < 2 && m_Rigidbody.velocity.magnitude < 0.5f;
        return x;
    }

    Vector3 PredictFinalPoint(Vector3 dv)
    {
        float timestep = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
        float velocity = m_Rigidbody.velocity.magnitude;
        float frameDrag = 1 - timestep * (m_Rigidbody.drag + dv.magnitude);
        if (velocity <= 0.1f)
            return Vector3.zero;

        int n = (int) (1f / Mathf.Log(frameDrag, 0.1f / velocity));
        float sumn = timestep * velocity * (Mathf.Pow(frameDrag, n) - 1) / (frameDrag - 1);
        return transform.position + m_Rigidbody.velocity.normalized * sumn;
    }
}