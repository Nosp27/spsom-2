using UnityEngine;

public class MissleLauncher : Weapon
{
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private float fireRate = 20;

    private Transform m_Target;
    private AudioSource audioSource;
    private float maxCooldown;
    private float cooldown;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
                e.DetonateForDistance(target.transform, 2);
            e.DetonateForTime(10);
        }
        
        if (audioSource != null)
            audioSource.Play();
    }
}
