using UnityEngine;

public class TrailEngineRenderer : EngineRenderer
{
    public TrailRenderer tr;
    [SerializeField] private int emissionThreshold;
    

    protected override void Die()
    {
        tr.enabled = false;
    }

    public override void SetThrust(int percent)
    {
        thrust = percent;
        percent = Mathf.Clamp(percent, 0, 100);
        tr.emitting = percent > emissionThreshold;
    }
}