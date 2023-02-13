using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using UnityEngine;


public enum HitType
{
    KINETIC,
    EXPLOSION,
    LASER,
}

[Serializable]
public class BulletHitDTO
{
    public int Damage;
    public HitType hitType;
    public Nullable<Vector3> HitDirection;
    public Nullable<Vector3> HitPoint;
    public GameObject hitInitiator;

    public BulletHitDTO(int damage, Vector3 hitPoint, Vector3 hitDirection, HitType ht = HitType.KINETIC, GameObject hitInitiator = null)
    {
        this.Damage = damage;
        this.HitDirection = hitDirection;
        this.HitPoint = hitPoint;
        this.hitType = ht;
        this.hitInitiator = hitInitiator;
    }
}