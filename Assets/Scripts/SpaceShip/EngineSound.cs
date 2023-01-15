using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleEngineRenderer))]
[RequireComponent(typeof(AudioSource))]
public class EngineSound : MonoBehaviour
{
    private ParticleEngineRenderer engine;

    private float lowThrottle = 0.2f;
    private float highThrottle = 0.8f;
    private float thrust => engine.thrust;
    [SerializeField] private AudioClip engineLow;
    [SerializeField] private AudioClip engineHigh;
    private AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = engineLow;

        engine = GetComponent<ParticleEngineRenderer>();
    }

    void Die()
    {
        source.Stop();
    }
    
    void Update()
    {
        bool shouldPlayLow = (thrust > lowThrottle && thrust < highThrottle);
        bool shouldPlayHigh = thrust > highThrottle;

        if (shouldPlayLow)
        {
            source.clip = engineLow;
        } else if (shouldPlayHigh)
        {
            source.clip = engineHigh;
        }
        
        if (!source.isPlaying && (shouldPlayHigh || shouldPlayLow))
        {
            source.Play();
        }
        
        if (source.isPlaying && !(shouldPlayHigh || shouldPlayLow))
        {
            source.Stop();
        }
    }
}