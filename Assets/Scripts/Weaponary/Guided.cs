using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Guided : MonoBehaviour
{
    public Transform Target;
    [SerializeField] private float MaxGuidingAngle;
    [SerializeField] private float Speed;
    private Rigidbody rb;

    public float PredictionMultiplier;
    private float StartingDistance;

    private GameObject targetMark;

    // Start is called before the first frame update
    void Start()
    {
        targetMark = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetMark.GetComponent<Collider>().enabled = false;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = transform.forward * Speed;
        if (Target)
        {
            Vector3 path = Target.position - transform.position;
            if (StartingDistance == 0)
            {
                StartingDistance = path.magnitude;
            }

            path += Target.GetComponentInParent<Rigidbody>().velocity *
                    Mathf.Lerp(0, PredictionMultiplier, path.magnitude / StartingDistance);

            Quaternion targetRotation = Quaternion.LookRotation(path);
            targetRotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, MaxGuidingAngle * Time.deltaTime);
            targetMark.transform.position = transform.position + path;
            rb.MoveRotation(targetRotation);
        }
    }
}