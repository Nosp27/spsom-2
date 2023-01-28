using System;
using UnityEngine;

public abstract class DamageModel : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int health = 0;

    private void Awake()
    {
        if (health == 0)
            health = maxHealth;
    }

    public int MaxHealth => maxHealth;
    public int Health => health;

    public abstract void Die();

    public abstract void GetDamage(BulletHitDTO hit);
}
