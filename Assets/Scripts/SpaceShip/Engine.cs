using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public ParticleSystem ps;

    public int ParticleIdleEmission;
    public int ParticleMaxEmission;

    public int ParticleIdleSpeed;
    public int ParticleMaxSpeed;

    public float DriveMultiplier;
    public float RotationMultiplier;

    [Range(0, 100)] public int thrust = 5;
    private int prev;

    [SerializeField] public bool debug = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Die()
    {
        ps.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (thrust != prev)
        {
            SetThrust(thrust);
            prev = thrust;
        }
    }

    public void SetThrust(int percent)
    {
        percent = Mathf.Clamp(percent, 0, 100);
        var emission = ps.emission;
        var main = ps.main;
        emission.rateOverTime = (float) (ParticleIdleEmission) +
                                (float) (ParticleMaxEmission - ParticleIdleEmission) / 100 * percent;
        main.startSpeed = (float) (ParticleIdleSpeed) + (float) (ParticleMaxSpeed - ParticleIdleSpeed) / 100 * percent;
        thrust = percent;
    }
}