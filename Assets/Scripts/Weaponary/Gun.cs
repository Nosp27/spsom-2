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
    [SerializeField] private GameObject overrideOwner;
    private int nBarrels => barrels.Length;

    public float maxCooldown { get; private set; }
    public float cooldown { get; private set; }

    private GameObject Owner;
    private int shootingBarrelIndex;

    private Vector3 GetVelocityCorrection(Transform barrel)
    {
        if (attachedRigidbody == null)
            return Vector3.zero;
        return attachedRigidbody.GetPointVelocity(barrel.position);
    }

    private Rigidbody attachedRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        InitBarrels();
        if (overrideOwner != null)
        {
            Owner = overrideOwner;
        }
        else
        {
            Owner = GetComponentInParent<Ship>().gameObject;
            attachedRigidbody = Owner.GetComponent<Rigidbody>();
        }

        cooldown = 0;
        maxCooldown = 60f / FireRate;
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
        b.Shoot(
            Owner, DamageBuff, b.transform.forward * BulletSpeed + GetVelocityCorrection(bulletPlace),
            BulletMaxDistance
        );
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