using UnityEngine;
using Random = UnityEngine.Random;

public class ShipDamageModel : DamageModel
{
    public ParticleSystem destruction;
    private Ship ship;

    [SerializeField] private bool destroyOnDeath;

    public GameObject[] HitParticleSystems;

    public GameObject[] ExplosionParticleSystems;

    [SerializeField] private AudioSource referenceAudioSource;
    [SerializeField] private AudioClip audioTakeDamage;
    [SerializeField] private AudioClip audioDie;

    [Space(20f)] [SerializeField] private GameObject aliveMesh;
    [SerializeField] private GameObject debrisMesh;

    private void Start()
    {
        ship = GetComponent<Ship>();
    }

    void PlayFX(AudioClip clip)
    {
        if (clip && referenceAudioSource)
        {
            AudioSource newAs = Instantiate(referenceAudioSource);
            newAs.transform.position = transform.position;
            newAs.clip = clip;
            newAs.Play();
            Destroy(newAs.gameObject, newAs.clip.length);
        }
    }

    public override void Die()
    {
        health = 0;
        if (ship.isPlayerShip)
            GameController.Current.SendMessage("Die");
        if (destruction != null)
            destruction.Play(true);

        PlayFX(audioDie);
        PlayDebris();

        if (destroyOnDeath)
            Destroy(gameObject, 0.5f);
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
        if (hit.Damage < 0)
        {
            Debug.LogWarning($"Damage <0: {hit.Damage}");
            return;
        }

        health -= hit.Damage;

        GameObject[] hitsParticles = HitParticleSystems;
        if (hit.hitType == HitType.EXPLOSION)
            hitsParticles = ExplosionParticleSystems;

        if (hitsParticles != null && hitsParticles.Length > 0 && hit.HitDirection.HasValue && hit.HitPoint.HasValue)
        {
            GameObject hpsPref = hitsParticles[Random.Range(0, hitsParticles.Length)];
            GameObject hps = Instantiate(hpsPref, hit.HitPoint.Value, Quaternion.LookRotation(-hit.HitDirection.Value),
                transform);
        }

        if (ship.isPlayerShip)
            GameController.Current.SendMessage("GetDamage", hit);
        if (health <= 0)
        {
            health = 0;
            if (destruction != null)
                destruction.Play(true);
            BroadcastMessage("Die");
        }
        else
        {
            if (hit.hitType != HitType.LASER)
                PlayFX(audioTakeDamage);
        }
    }
}