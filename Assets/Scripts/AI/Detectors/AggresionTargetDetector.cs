using System.Collections;
using GameEventSystem;
using UnityEngine;

public class AggresionTargetDetector : MonoBehaviour, ITargetDetector
{
    public DamageModel Target { get; private set; }
    [SerializeField] private float memory = 10f;

    private Coroutine resetCoro = null;

    void Start()
    {
        EventLibrary.objectReceivesDamage.AddListener(OnDamage);
    }

    void OnDamage(DamageModel damaged, BulletHitDTO hit)
    {
        if (damaged != GetComponent<Ship>())
        {
            return;
        }
        
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