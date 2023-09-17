using System;
using GameEventSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipDamageModel : DamageModel
{
    public ParticleSystem destruction;

    [SerializeField] private bool destroyOnDeath;

    public GameObject[] HitParticleSystems;

    public GameObject[] ExplosionParticleSystems;

    private BulletHitDTO lastHitDTO;

    [SerializeField] private bool debug;

    public override void Die()
    {
        health = 0;
        if (destruction != null)
            destruction.Play(true);

        PlayDebris();

        if (destroyOnDeath)
            Destroy(gameObject, 0.5f);
        base.Die();
    }

    public void ChangeMaxHealth(int newMaxHealth)
    {
        float healthPercent = 1.0f * health / maxHealth;
        maxHealth = newMaxHealth;
        int newHealth = (int)(maxHealth * healthPercent);
        HealDamage(newHealth - health);
    }

    protected override void PlayDebris()
    {
        if (!(AliveMesh && DebrisMesh))
            return;

        base.PlayDebris();

        if (lastHitDTO.HitDirection.HasValue)
        {
            foreach (Rigidbody rb in DebrisMesh.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddForce(lastHitDTO.HitDirection.Value.normalized * lastHitDTO.Damage * 300);
            }
        }
    }

    public void HealDamage(int healAmount)
    {
        if (healAmount < 0)
        {
            throw new Exception("Heal with negative amount error");
        }
        if (healAmount == 0)
            return;
        
        health = Mathf.Min(health + healAmount, maxHealth);
        EventLibrary.shipReceivesHeal.Invoke(this);
    }

    public override void GetDamage(BulletHitDTO hit)
    {
        if (!Alive)
            return;

        if (hit.Damage < 0)
        {
            Debug.LogWarning($"Damage <0: {hit.Damage}");
            return;
        }

        if (debug)
            print($"{name} recieves damage");

        lastHitDTO = hit;
        
        bool deadly = false;
        health -= hit.Damage;
        if (health <= 0)
        {
            deadly = true;
            health = 0;
        }

        GameObject[] hitsParticles = HitParticleSystems;
        if (hit.hitType == HitType.EXPLOSION)
            hitsParticles = ExplosionParticleSystems;

        if (hitsParticles != null && hitsParticles.Length > 0 && hit.HitDirection.HasValue && hit.HitPoint.HasValue)
        {
            GameObject hpsPref = hitsParticles[Random.Range(0, hitsParticles.Length)];
            GameObject hps = Instantiate(hpsPref, hit.HitPoint.Value, Quaternion.LookRotation(-hit.HitDirection.Value),
                transform);
        }

        if (deadly)
        {
            if (destruction != null)
                destruction.Play(true);
            EventLibrary.shipKills.Invoke(
                hit.hitInitiator.GetComponent<Ship>(),
                GetComponent<DamageModel>()
            );
            Die();
        }

        base.GetDamage(hit);
    }
}