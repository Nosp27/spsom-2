using FMODUnity;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    LayerMask mask;

    public float Speed;

    public int Damage = 3;

    public GameObject Owner;
    private Rigidbody rb;

    private StudioEventEmitter eventEmitter;

    private bool hitDone = false;
    [SerializeField] private bool bypassShields;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        eventEmitter = GetComponentInChildren<StudioEventEmitter>();
        mask = LayerMask.GetMask(new[]
        {
            "Default"
        });
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

        if (other.gameObject.transform.IsChildOf(Owner.transform))
        {
            return;
        }

        Hit();
        eventEmitter.Play();

        Vector3 collisionPoint =
            transform.position + transform.forward * GetComponentInChildren<Collider>().bounds.size.z;
        Vector3 hitDirection = transform.forward;
        BulletHitDTO hit = new BulletHitDTO(Damage, collisionPoint, hitDirection, HitType.KINETIC, Owner);

        if (other.TryGetComponent(out Shield shield) && !bypassShields)
        {
            shield.SendMessage("GetDamage", hit);
            return;
        }
        
        DamageModel damageModel = other.GetComponentInParent<DamageModel>();
        if (damageModel && damageModel.gameObject != Owner)
        {
            damageModel.SendMessage("GetDamage", hit);
        }
    }


    void Die()
    {
        Destroy(gameObject, 0.01f);
    }

    void Hit()
    {
        Destroy(gameObject, 0.01f);
    }

    protected virtual float GetDamage()
    {
        return Damage;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.forward * Speed;
    }
}