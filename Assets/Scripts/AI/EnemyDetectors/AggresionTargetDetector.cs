using System.Collections;
using UnityEngine;

public class AggresionTargetDetector : MonoBehaviour, ITargetDetector
{
    public DamageModel Target { get; private set; }
    [SerializeField] private float memory = 10f;

    private Coroutine resetCoro = null;

    void Start()
    {
        DamageModel damageModel = GetComponentInParent<DamageModel>();
        damageModel.OnDamage.AddListener(OnDamage);
    }

    void OnDamage(BulletHitDTO hit)
    {
        Target = hit.hitInitiator?.GetComponentInParent<DamageModel>();
        if (!Target)
            return;
        
        if (resetCoro != null)
            StopCoroutine(resetCoro);
        resetCoro = StartCoroutine(ResetEnemy());
    }

    IEnumerator ResetEnemy()
    {
        yield return new WaitForSeconds(memory);
        Target = null;
    }
}