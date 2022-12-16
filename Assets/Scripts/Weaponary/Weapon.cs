using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract void Track(Transform target);
    public abstract void Aim(Vector3 target);

    public abstract bool Aimed();
    public abstract void Fire();

    public void FireIfAimed()
    {
        if (Aimed())
            Fire();
    }
}