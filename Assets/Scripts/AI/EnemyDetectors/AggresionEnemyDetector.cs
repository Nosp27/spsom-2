using System.Collections;
using UnityEngine;

public class AggresionEnemyDetector : MonoBehaviour, IEnemyDetector
{
    public DamageModel Enemy { get; private set; }
    [SerializeField] private float memory = 10f;

    private Coroutine resetCoro = null;

    void Start()
    {
        DamageModel damageModel = GetComponentInParent<DamageModel>();
        damageModel.OnDamage.AddListener(OnDamage);
    }

    void OnDamage(BulletHitDTO hit)
    {
        Enemy = hit.hitInitiator?.GetComponentInParent<DamageModel>();
        if (!Enemy)
            return;
        
        if (resetCoro != null)
            StopCoroutine(resetCoro);
        resetCoro = StartCoroutine(ResetEnemy());
    }

    IEnumerator ResetEnemy()
    {
        yield return new WaitForSeconds(memory);
        Enemy = null;
    }
}