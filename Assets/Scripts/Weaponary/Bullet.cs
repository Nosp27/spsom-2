using System;
using FMODUnity;
using GameEventSystem;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    LayerMask mask;
    private float Speed;
    private GameObject m_Owner;
    private Rigidbody rb;
    private StudioEventEmitter eventEmitter;
    private bool hitDone = false;

    private Collider bulletTrigger;

    [SerializeField] private int Damage = 3;
    [SerializeField] private bool bypassShields;
    [SerializeField] private GameObject hitEffectPrefab;

    [SerializeField] private bool debug;

    private void Start()
    {
        if (rb != null)
            return;

        rb = GetComponent<Rigidbody>();
        bulletTrigger = GetComponentInChildren<Collider>();
        eventEmitter = GetComponentInChildren<StudioEventEmitter>();
        mask = LayerMask.GetMask(new[]
        {
            "Default"
        });
    }

    public void Shoot(GameObject owner, float damageBuff, Vector3 bulletVelocity, float bulletMaxDistance)
    {
        if (rb == null)
            Start();

        if (m_Owner != null)
            throw new Exception("Bullet was already inited");
        m_Owner = owner;
        Damage = (int)(Damage * damageBuff);
        Speed = bulletVelocity.magnitude;
        rb.velocity = bulletVelocity;
        Destroy(gameObject, bulletMaxDistance / Speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.GetComponent<Bullet>())
        {
            return;
        }

        if ((mask & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        if (other.gameObject.transform.IsChildOf(m_Owner.transform))
        {
            return;
        }

        Hit();
        eventEmitter.Play();

        Vector3 collisionPoint =
            transform.position + transform.forward * GetComponentInChildren<Collider>().bounds.size.z;
        Vector3 hitDirection = transform.forward;
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, collisionPoint, Quaternion.LookRotation(-hitDirection));
        }
        BulletHitDTO hit = new BulletHitDTO(Damage, collisionPoint, hitDirection, HitType.KINETIC, m_Owner);

        EventLibrary.shipDealsDamage.Invoke(m_Owner.GetComponent<Ship>(), hit);
        
        DamageModel damageModel = other.GetComponentInParent<DamageModel>();
        if (damageModel)
        {
            EventLibrary.objectReceivesDamage.Invoke(damageModel, hit);
        }
    }

    private void FixedUpdate()
    {
        var p = bulletTrigger.transform.position;
        p.y = 0;
        bulletTrigger.transform.position = p;
    }


    void Die()
    {
        Destroy(gameObject, 0.01f);
    }

    void Hit()
    {
        Destroy(gameObject, 0.01f);
    }
}