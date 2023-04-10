using SpaceShip.PhysicalMovement;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicalMovement : MonoBehaviour
{
    public Vector3 MoveAim { get; private set; }

    private Rigidbody m_Rigidbody;
    private PhysicalEngineSplitter m_Splitter;

    [SerializeField] private Transform testTarget;
    [SerializeField] private Transform testIndicator;

    private bool breaking = false;
    
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Splitter = GetComponent<PhysicalEngineSplitter>();
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            breaking = false;
            MoveAim = testTarget.position;
        }

        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            breaking = true;
        }

        testIndicator.transform.position = PredictFinalPoint(m_Splitter.CalculateForce(-m_Rigidbody.velocity));
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position + Vector3.up * 5, m_Splitter.CalculateForce(testTarget.position - transform.position), Color.green);
        if (MoveAim != Vector3.zero && !At(MoveAim))
        {
            MoveStep();
            m_Splitter.ApplyRotationTorque(MoveAim-transform.position);
        }
        else
        {
            breaking = false;
            MoveAim = Vector3.zero;
        }
    }

    void MoveStep()
    {
        Vector3 point = MoveAim - transform.position;
        Vector3 velocity = m_Rigidbody.velocity;
        
        Vector3 deltaV = Vector3.zero;

        // Reduce side velocity
        Vector3 sideVelocity = velocity - Utils.Projection(velocity, point) * velocity.normalized;
        if (sideVelocity.magnitude > .1f)
        {
            m_Splitter.ApplyDeltaV(-sideVelocity);
            return;
        }
        
        Vector3 breakingVector = -point;
        Vector3 breakingPoint = PredictFinalPoint(m_Splitter.CalculateForce(breakingVector));
        if (Vector3.Distance(breakingPoint, MoveAim) < .5f)
        {
            m_Splitter.ApplyDeltaV(-velocity);
        }
        else
        {
            m_Splitter.ApplyDeltaV(point);
        }
    }

    bool At(Vector3 point)
    {
        return Vector3.Distance(point, transform.position) < 2 && m_Rigidbody.velocity.magnitude < 0.5f;
    }

    Vector3 PredictFinalPoint(Vector3 dv)
    {
        float timestep = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
        float velocity = m_Rigidbody.velocity.magnitude;
        float frameDrag = 1 - timestep * (m_Rigidbody.drag + dv.magnitude);
        if (velocity <= 0.1f)
            return Vector3.zero;

        int n = (int)(1f / Mathf.Log(frameDrag, 0.1f / velocity));
        float sumn = timestep * velocity * (Mathf.Pow(frameDrag, n) - 1) / (frameDrag - 1);
        return transform.position + m_Rigidbody.velocity.normalized * sumn;
    }
}