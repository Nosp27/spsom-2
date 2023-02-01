using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(EngineRenderer))]
[RequireComponent(typeof(StudioEventEmitter))]
public class EngineSound : MonoBehaviour
{
    private EngineRenderer engine;
    private float thrust => engine.thrust;
    [SerializeField] private StudioEventEmitter engineEvent;

    void Start()
    {
        engine = GetComponent<EngineRenderer>();
    }

    void Die()
    {
        engineEvent.Stop();
    }
    
    void Update()
    {
        if (thrust > 5 && !engineEvent.IsPlaying())
        {
            engineEvent.Play();
        }
        engineEvent.SetParameter("Power", 0.01f * thrust);
    }
}