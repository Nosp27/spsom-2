using FMODUnity;
using UnityEngine;

public class MissleLauncher : Weapon
{
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float fireRate = 20;
    [SerializeField] private float detonationDistance = 2;
    [SerializeField] private float detonationTimeout = 10;

    public override float maxCooldown { get; protected set; }
    public override float cooldown { get; protected set; }
    
    private Transform m_Target;

    private void Start()
    {
        maxCooldown = 60 / fireRate;
    }

    void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
    }

    public override void Track(Transform t)
    {
        m_Target = t;
    }

    public override void Aim(Vector3 target)
    {
        // Only can track targets
    }

    public override bool Aimed()
    {
        return m_Target != null;
    }

    public override void Fire()
    {
        if (cooldown <= 0)
        {
            cooldown = maxCooldown;
            LaunchMissile(m_Target);
        }
    }

    private void LaunchMissile(Transform target)
    {
        GameObject missile = Instantiate(missilePrefab, transform.position, transform.rotation);
        Guided g = missile.GetComponent<Guided>();
        Explosive e = missile.GetComponent<Explosive>();

        if (g && target)
            g.Target = target;

        if (e)
        {
            if(target)
                e.DetonateForDistance(target.transform, detonationDistance);
            e.DetonateForTime(detonationTimeout);
        }
        shotEventEmitter.Play();
    }
}
