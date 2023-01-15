using UnityEngine;

public class ParticleEngineRenderer : EngineRenderer
{
    public ParticleSystem ps;

    [SerializeField] private int ParticleIdleEmission;
    [SerializeField] private int ParticleMaxEmission;
    [SerializeField] private float ParticleIdleSpeed;
    [SerializeField] private float ParticleMaxSpeed;

    [SerializeField] private bool useEmissionOverDistance;


    protected override void Die()
    {
        ps.Stop();
    }

    public override void SetThrust(int percent)
    {
        percent = Mathf.Clamp(percent, 0, 100);
        var emission = ps.emission;
        var main = ps.main;
        
        float emissionValue = Mathf.Lerp(ParticleIdleEmission, ParticleMaxEmission, percent / 100f);
        if (useEmissionOverDistance)
        {
            emission.rateOverDistance = emissionValue;
        }
        else
        {
            emission.rateOverTime = emissionValue;
        }
        main.startSpeed = Mathf.Lerp(ParticleIdleSpeed, ParticleMaxSpeed, percent / 100f);
    }
}