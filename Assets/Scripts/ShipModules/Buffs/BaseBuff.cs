using UnityEngine;

public abstract class BaseBuff : MonoBehaviour
{
    public bool WasApplied { get; private set; }

    private GameObject appliedOn;

    public void Apply(GameObject go)
    {
        if (WasApplied)
            return;
        
        if (ApplyBuffInternal(go))
        {
            appliedOn = go;
            WasApplied = true;
        }
    }

    protected abstract bool ApplyBuffInternal(GameObject go);

    public void Cancel()
    {
        if (!WasApplied)
            return;

        if (CancelBuffInternal(appliedOn))
        {
            WasApplied = false;
            appliedOn = null;
        }
    }

    protected virtual bool CancelBuffInternal(GameObject go)
    {
        return false;
    } 
}
