using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShipDamageModel : MonoBehaviour
{
    public int MaxHealth { get; private set; }
    public int Health;
    public ParticleSystem destruction;
    private Ship ship;

    public GameObject[] HitParticleSystems;

    public GameObject[] ExplosionParticleSystems;

    [SerializeField] private AudioSource referenceAudioSource;
    [SerializeField] private AudioClip audioTakeDamage;
    [SerializeField] private AudioClip audioDie;

    private void Start()
    {
        MaxHealth = Health;
        ship = GetComponent<Ship>();
    }

    void PlayFX(AudioClip clip)
    {
        if (clip && referenceAudioSource)
        {
            AudioSource newAs = Instantiate(referenceAudioSource);
            newAs.clip = clip;
            newAs.Play();
            Destroy(newAs.gameObject, newAs.clip.length);
        }
    }

    public void Die()
    {
        Health = 0;
        if (ship.isPlayerShip)
            GameController.Current.SendMessage("Die");
        if (destruction != null)
            destruction.Play(true);
        
        print("PlayDIe");
        PlayFX(audioDie);
    }

    public void GetDamage(BulletHitDTO hit)
    {
        if (hit.Damage < 0)
        {
            print($"Damage <0: {hit.Damage}");
            return;
        }

        Health -= hit.Damage;

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
        if (Health <= 0)
        {
            Health = 0;
            if (destruction != null)
                destruction.Play(true);
            BroadcastMessage("Die");
        }
        else
        {
            PlayFX(audioTakeDamage);
        }
    }
}