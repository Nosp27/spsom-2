using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    public float BulletSpeed;
    [SerializeField] Transform BulletPlace;
    [SerializeField] float BulletMaxDistance;
    [SerializeField] private float _bulletLifetime;
    [SerializeField] float FireRate;
    [SerializeField] float DamageBuff;

    private float maxCooldown;
    private float cooldown;

    private GameObject Owner;

    // Start is called before the first frame update
    void Start()
    {
        Owner = GetComponentInParent<Ship>().gameObject;
        cooldown = 0;
        maxCooldown = 60f / FireRate;
        _bulletLifetime = BulletMaxDistance / BulletSpeed;
    }

    private void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;   
        }

        if (cooldown < 0)
        {
            cooldown = 0;
        }
    }

    // Update is called once per frame
    public void Shoot()
    {
        if (cooldown > 0)
            return;
        GameObject bullet = Instantiate(bulletPrefab, BulletPlace.position, BulletPlace.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Owner = Owner;
        b.Damage = (int) (b.Damage * DamageBuff);
        b.Speed = BulletSpeed;
        Destroy(bullet.gameObject, _bulletLifetime);
        cooldown = maxCooldown;
    }
}
