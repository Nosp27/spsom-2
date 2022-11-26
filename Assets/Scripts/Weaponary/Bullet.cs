using UnityEngine;

public class Bullet : MonoBehaviour
{
    LayerMask mask;

    public float Speed;

    public int Damage = 3;

    public GameObject Owner;
    private Rigidbody rb;

    private AudioSource audioSource;

    private bool hitDone = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponentInChildren<AudioSource>();
        mask = LayerMask.GetMask(new[]
        {
            "UI"
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && other.attachedRigidbody.GetComponent<Bullet>())
        {
            return;
        }

        if ((mask & (1 << other.gameObject.layer)) != 0)
        {
            return;
        }
        
        Hit();

        LinUtils.PlayAudioDetached(audioSource);

        ShipDamageModel damageModel = other.GetComponentInParent<ShipDamageModel>();
        Vector3 collisionPoint =
            transform.position + transform.forward * GetComponentInChildren<Collider>().bounds.size.z;
        Vector3 hitDirection = transform.forward;
        BulletHitDTO hit = new BulletHitDTO(Damage, collisionPoint, hitDirection);
        if (damageModel && damageModel.gameObject != Owner)
        {
            print($"DD: {hit.Damage}");
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