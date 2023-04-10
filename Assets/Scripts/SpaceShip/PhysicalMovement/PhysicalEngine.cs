using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalEngine : MonoBehaviour
{
    public EmissionEngineRenderer renderer;
    public float maxPower;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<EmissionEngineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
