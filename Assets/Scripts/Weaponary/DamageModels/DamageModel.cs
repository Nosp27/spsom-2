using UnityEngine;
using UnityEngine.Events;

public abstract class DamageModel : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int health = 0;

    public UnityEvent<BulletHitDTO> OnDamage { get; private set; }
    public UnityEvent OnDie { get; private set; }

    private void Awake()
    {
        OnDamage = new UnityEvent<BulletHitDTO>();
        OnDie = new UnityEvent();
        alive = true;
        if (health == 0)
            health = maxHealth;
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
        OnDie.Invoke();
    }

    public virtual void GetDamage(BulletHitDTO hit)
    {
        if (!alive)
            OnDamage.Invoke(hit);
    }
}