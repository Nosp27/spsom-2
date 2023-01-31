using System;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    enum ShootMode
    {
        RR,
        RANDOM
    }

    [SerializeField] private StudioEventEmitter sfxEmitter;
    [SerializeField] private ShootMode shootMode;
    [SerializeField] GameObject bulletPrefab;
    public float BulletSpeed;
    [SerializeField] float BulletMaxDistance;
    [SerializeField] float FireRate;
    [SerializeField] float DamageBuff;

    [SerializeField] private Transform[] barrels;
    private int nBarrels => barrels.Length;

    public float maxCooldown { get; private set; }
    public float cooldown { get; private set; }

    private GameObject Owner;
    private int shootingBarrelIndex;
    private float _bulletLifetime;

    // Start is called before the first frame update
    void Start()
    {
        InitBarrels();
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
        int barrelIndex = ChooseBarrelToShoot();
        Transform bulletPlace = barrels[barrelIndex];
        GameObject bullet = Instantiate(bulletPrefab, bulletPlace.position, bulletPlace.rotation);
        Bullet b = bullet.GetComponent<Bullet>();
        b.Owner = Owner;
        b.Damage = (int) (b.Damage * DamageBuff);
        b.Speed = BulletSpeed;
        Destroy(bullet.gameObject, _bulletLifetime);
        cooldown = maxCooldown;
        if (sfxEmitter)
            sfxEmitter.Play();
    }

    void InitBarrels()
    {
        if (barrels == null || barrels.Length == 0)
        {
            barrels = new[] {transform};
        }
    }

    int ChooseBarrelToShoot()
    {
        if (nBarrels == 1)
        {
            return 0;
        }

        if (shootMode == ShootMode.RR)
        {
            return shootingBarrelIndex++ % nBarrels;
        }

        if (shootMode == ShootMode.RANDOM)
        {
            return Random.Range(0, nBarrels);
        }

        throw new Exception("Unknown barrel resolve method");
    }
}