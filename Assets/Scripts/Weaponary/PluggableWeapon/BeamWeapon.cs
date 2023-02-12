using UnityEngine;

public class BeamWeapon : Weapon
{
    [SerializeField] private float damageRatePerSecond = 40;
    [SerializeField] private float range = 200;
    [SerializeField] private LineRenderer beamRenderer;
    [SerializeField] private ParticleSystem startEffect;
    [SerializeField] private ParticleSystem endEffect;

    public override float maxCooldown { get; protected set; }
    public override float cooldown { get; protected set; }

    private Vector3 aimTarget;

    private bool _beamOn;
    private float _beamDissolveAccumulator;
    private float _beamDissolveLatency = .1f;
    private Rigidbody attachedRigidbody;
    private DamageModel attackedDamageModel;
    private float damageCounter;

    void Start()
    {
        attachedRigidbody = GetComponentInParent<Rigidbody>();
        SwitchBeam(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha2))
        {
            Fire();
        }

        if (_beamOn)
        {
            ProcessBeam();
            _beamDissolveAccumulator += Time.deltaTime;
            if (_beamDissolveAccumulator > _beamDissolveLatency)
            {
                SwitchBeam(false);
            }
        }

        if (endEffect && beamRenderer.enabled)
        {
            endEffect.transform.position = beamRenderer.GetPosition(1);
            beamRenderer.SetPosition(0, beamRenderer.transform.position);
        }
    }

    public override void Track(Transform target)
    {
    }

    public override void Aim(Vector3 target)
    {
        aimTarget = target;
    }

    public override bool Aimed()
    {
        Vector3 lookVector = transform.forward;
        Vector3 targetVector = aimTarget - transform.position;
        float angle = Vector3.Angle(lookVector, targetVector);
        return angle < 5f;
    }

    public override void Fire()
    {
        _beamDissolveAccumulator = 0;
        if (!_beamOn)
            SwitchBeam(true);
    }

    private void SwitchBeam(bool on)
    {
        _beamOn = on;
        beamRenderer.enabled = on;


        if (on)
        {
            startEffect.Play();
            if (endEffect)
                endEffect.Play();
        }
        else
        {
            startEffect.Stop();
            if (endEffect)
                endEffect.Stop();
        }
    }

    private void ProcessBeam()
    {
        RaycastHit hit;
        if (PerformRaycast(out hit))
        {
            Vector3 hitPoint = hit.point;
            beamRenderer.SetPosition(1, hitPoint);
            DamageModel damageModel = hit.collider.GetComponentInParent<DamageModel>();
            if (hit.rigidbody != attachedRigidbody)
            {
                if (attackedDamageModel == damageModel)
                {
                    damageCounter += damageRatePerSecond * Time.deltaTime;
                    if (damageCounter >= 1 && damageModel)
                    {
                        damageModel.SendMessage(
                            "GetDamage",
                            new BulletHitDTO(
                                (int)damageCounter,
                                hitPoint,
                                (hitPoint - transform.position).normalized,
                                HitType.LASER
                            )
                        );
                        damageCounter = 0;
                    }
                }
                else
                {
                    damageCounter = 0;
                    attackedDamageModel = damageModel;   
                }
            }
        }
        else
        {
            beamRenderer.SetPosition(1, transform.position + range * transform.forward);
        }
    }

    private bool PerformRaycast(out RaycastHit hit)
    {
        return Physics.Raycast(
            transform.position,
            transform.forward,
            out hit,
            range,
            LayerMask.GetMask("Default"),
            QueryTriggerInteraction.Ignore
        );
    }
}