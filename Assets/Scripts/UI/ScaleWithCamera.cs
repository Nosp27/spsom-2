using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithCamera : MonoBehaviour
{
    private Camera cam;

    [SerializeField] float Multiplier;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * (cam.transform.position - transform.position).magnitude * Multiplier / 100f;
    }
}
