using UnityEngine;

public abstract class EngineRenderer : MonoBehaviour
{
    [Range(0, 100)] public int thrust = 5;
    private int prev;
    
    [SerializeField] public bool debug = false;

    protected abstract void Die();

    // Update is called once per frame
    protected virtual void Update()
    {
        if (thrust != prev)
        {
            SetThrust(thrust);
            prev = thrust;
        }
    }

    public abstract void SetThrust(int percent);
}