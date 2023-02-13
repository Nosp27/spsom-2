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
        if (health == 0)
            health = maxHealth;
    }

    public int MaxHealth => maxHealth;
    public int Health => health;

    public virtual void Die()
    {
        OnDie.Invoke();
    }

    public virtual void GetDamage(BulletHitDTO hit)
    {
        OnDamage.Invoke(hit);
    }
}
