using System;
using UnityEngine;

public class EmissionEngineRenderer : MonoBehaviour
{
    private ParticleSystem ps;

    [SerializeField] private AnimationCurve lifetimeMultiplier = AnimationCurve.Linear(0, 0.1f, 100, 4f);
    [SerializeField] private AnimationCurve emission = AnimationCurve.Linear(0, 1, 100, 160);
    [SerializeField] private AnimationCurve startSizeMultiplier = AnimationCurve.Linear(0, 1, 100, 0.4f);

    ParticleSystem.MainModule m_MainModule;
    ParticleSystem.EmissionModule m_EmissionModule;

    private float maxResetTimeout = 0.1f;
    private float resetTimeout;
    private float m_SizeConstant;
    void Start()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        m_MainModule = ps.main;
        m_EmissionModule = ps.emission;
        m_SizeConstant = ps.main.startSizeMultiplier;
    }

    private void FixedUpdate()
    {
        if (resetTimeout > 0)
        {
            resetTimeout -= Time.fixedDeltaTime;
        }
        else
        {
            ps.Stop(true);
        }
    }

    public void Perform(int percent)
    {
        if (percent > 0)
        {
            ps.Play(true);
            resetTimeout = maxResetTimeout;
        }
        else
        {
            ps.Stop(true);
        }
        
        
        m_MainModule.startLifetimeMultiplier = lifetimeMultiplier.Evaluate(percent);
        m_EmissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(emission.Evaluate(percent));
        m_MainModule.startSizeMultiplier = startSizeMultiplier.Evaluate(percent) * m_SizeConstant;
    }
}