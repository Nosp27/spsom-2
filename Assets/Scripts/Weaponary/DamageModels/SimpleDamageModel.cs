using UnityEngine;

public class SimpleDamageModel : DamageModel
{
    [SerializeField] private GameObject debrisGameObject;

    public override void Die()
    {
        base.Die();
        if (debrisGameObject)
        {
            debrisGameObject.SetActive(true);
        }
        Destroy(gameObject, 0.5f);
    }

    public override void GetDamage(BulletHitDTO hit)
    {
        base.GetDamage(hit);
        if (hit.Damage < 0)
        {
            Debug.LogWarning($"Damage <0: {hit.Damage}");
            return;
        }

        health -= hit.Damage;
        if (health <= 0)
        {
            Die();
        }
    }
}
