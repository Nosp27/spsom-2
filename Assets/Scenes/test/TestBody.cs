using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestBody : MonoBehaviour
{
    private Rigidbody m_Rigidbody;

    [SerializeField] private float brakingForce = 7f;
    [SerializeField] private Transform testIndicator;

    private bool brake;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.velocity = transform.forward * 8;
    }

    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
            brake = !brake;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 brakingVector = -1 * transform.forward * brakingForce;
        testIndicator.position = PredictFinalPointNoDrag(brakingVector);
        if (brake)
        {
            m_Rigidbody.AddForce(brakingVector * Time.deltaTime);
        }
    }
    
    Vector3 PredictFinalPointNoDrag(Vector3 f)
    {
        // frame duration
        float dt = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
        
        // dv
        float dv = f.magnitude / m_Rigidbody.mass * dt;
        
        // velocity
        float v = m_Rigidbody.velocity.magnitude;

        if (v <= 0.1f)
            return Vector3.zero;
        
        float i = (v / dv);
        float sumn = i * v / 2f;
        return transform.position + m_Rigidbody.velocity.normalized * sumn * dt;
    }
    
    Vector3 PredictFinalPoint(Vector3 f)
    {
        // frame duration
        float dt = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
        
        // dv
        float dv = f.magnitude / m_Rigidbody.mass * dt;
        
        // velocity
        float v = m_Rigidbody.velocity.magnitude;

        if (v <= 0.1f)
            return Vector3.zero;
        
        // drag for one frame
        float b = 1 - dt * m_Rigidbody.drag;

        // index of frame where velocity is `<=0.1f`
        int i = (int) Mathf.Log((dv * b / (b - 1)) / (v - (dv / (b - 1))), b);
        
        float sumn = 0f;
        
        // part1 (for drag)
        sumn += v * (Mathf.Pow(b, i - 1) - 1) / (b - 1);
        
        // part2 (for braking velocity)
        sumn -= dv * b * (Mathf.Pow(b, i - 2) - 1) / (b * b - 2 * b + 1);
        
        // part3
        sumn += i * dv * b / (b - 1);
        
        print($"i: {i}, {sumn}");
        
        return transform.position + m_Rigidbody.velocity.normalized * sumn * dt;
    }
}
