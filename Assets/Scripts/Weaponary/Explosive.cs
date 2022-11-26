using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float Range = 10;
    [SerializeField] private int MinDamage = 3;
    [SerializeField] private int MaxDamage = 10;

    private List<Predicate<Explosive>> detonationSensors = new List<Predicate<Explosive>>();
    private bool Detonated = false;

    private AudioSource audioSource;

    [SerializeField] private ParticleSystem Explosion;

    public void Explode()
    {
        print("E");
        if (Detonated)
            return;

        if (Explosion)
            Explosion.Play();

        Detonated = true;

        LinUtils.PlayAudioDetached(audioSource);

        Collider[] hits = Physics.OverlapSphere(transform.position, Range);

        Dictionary<ShipDamageModel, List<Collider>> hitMap = new Dictionary<ShipDamageModel, List<Collider>>();
        for (int i = 0; i < hits.Length; i++)
        {
            ShipDamageModel dm = hits[i].GetComponentInParent<ShipDamageModel>();
            if (dm != null)
            {
                if (!hitMap.ContainsKey(dm))
                {
                    hitMap[dm] = new List<Collider>();
                }

                hitMap[dm].Add(hits[i]);
            }
        }

        foreach (var item in hitMap)
        {
            ShipDamageModel dm = item.Key;
            float distance = (dm.transform.position - transform.position).magnitude;
            int Damage = (int) (MaxDamage - ((MaxDamage - MinDamage) * (distance / Range)));

            var dto = new BulletHitDTO(
                Damage,
                dm.transform.position,
                Vector3.down,
                HitType.EXPLOSION
            );
            dm.SendMessage("GetDamage", dto);
        }

        print("Destroy");
        Destroy(gameObject, .1f);
    }

    private void Update()
    {
        if (!Detonated)
        {
            foreach (var s in detonationSensors)
            {
                if (s.Invoke(this))
                    Explode();
            }
        }
    }

    public void DetonateForDistance(Transform target, float distance)
    {
        detonationSensors.Add(e => (target.transform.position - e.transform.position).magnitude < distance);
    }

    public void DetonateForTime(float timeout)
    {
        float ttl = Time.time + timeout;
        detonationSensors.Add(e => Time.time > ttl);
    }
}