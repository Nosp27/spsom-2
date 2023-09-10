using GameEventSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipDamageModel : DamageModel
{
    public ParticleSystem destruction;

    [SerializeField] private bool destroyOnDeath;

    public GameObject[] HitParticleSystems;

    public GameObject[] ExplosionParticleSystems;

    [Space(20f)] [SerializeField] private GameObject aliveMesh;
    [SerializeField] private GameObject debrisMesh;

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

    void PlayDebris()
    {
        if (!(aliveMesh && debrisMesh))
            return;

        aliveMesh.SetActive(false);
        debrisMesh.transform.parent = null;
        debrisMesh.SetActive(true);
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