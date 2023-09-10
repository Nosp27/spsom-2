using GameEventSystem;
using UnityEngine;
using UnityEngine.Events;

public abstract class DamageModel : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int health = 0;

    private void Awake()
    {
        alive = true;
        if (health == 0)
            health = maxHealth;
        EventLibrary.objectReceivesDamage.AddListener(DamageListener);
    }

    private bool alive;
    public bool Alive => alive;

    public int MaxHealth => maxHealth;
    public int Health => health;

    public virtual void Die()
    {
        if (!alive)
            return;

        alive = false;
        EventLibrary.objectDestroyed.Invoke(this);
    }

    private void DamageListener(DamageModel dm, BulletHitDTO dto)
    {
        if (dm == this)
        {
            GetDamage(dto);
        }
    }

    public virtual void GetDamage(BulletHitDTO hit)
    {
    }
}