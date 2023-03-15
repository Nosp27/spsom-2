using System;
using FMODUnity;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected StudioEventEmitter shotEventEmitter;
    [SerializeField] private Sprite weaponSpriteValue;
    public Sprite weaponSprite => weaponSpriteValue;
    public abstract float maxCooldown { get; protected set; }
    public abstract float cooldown { get; protected set; }

    private Ship rootShip;
    
    protected void OnEnable()
    {
        rootShip = GetComponentInParent<Ship>();
        if (rootShip)
            rootShip.ScanWeaponary();
    }

    private void OnTransformParentChanged()
    {
        if (rootShip)
            rootShip.ScanWeaponary();
    }

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