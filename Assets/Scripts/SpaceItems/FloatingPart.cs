using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingPart : MonoBehaviour
{
    private Rigidbody rb;

    public float BaseSpeed;

    public float SpeedDeviation;

    public float RotationSpeed;

    private float Speed;

    private Transform mesh;

    private Quaternion rotationStep;

    // Start is called before the first frame update
    void Start()
    {
        mesh = gameObject.GetComponentInChildren<Transform>();
        Speed = BaseSpeed + (2 * Random.value - 1) * SpeedDeviation;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.velocity = transform.forward * Speed;

        float rotationMultiplier = Time.deltaTime * RotationSpeed;
        rotationStep = Quaternion.Euler(Random.value * rotationMultiplier, Random.value * rotationMultiplier, Random.value * rotationMultiplier);
    }

    // Update is called once per frame
    void Update()
    {
        mesh.rotation *= rotationStep;
    }
}
