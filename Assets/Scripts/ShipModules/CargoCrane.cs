using System;
using FMODUnity;
using GameEventSystem;
using UI.Inventory;
using UnityEngine;
using UnityEngine.Events;

public enum CargoCraneInventoryResolveStrategy
{
    PLAYER_INVENTORY,
    ATTACHED_INVENTORY,
}

public class CargoCrane : MonoBehaviour
{
    [SerializeField] private StudioEventEmitter grabEventEmitter;
    [SerializeField] private StudioEventEmitter pullEventEmitter;
    [SerializeField] private float fetchSpeed = 10;
    [SerializeField] private float fetchRadius;
    [SerializeField] private float grabRadius;
    [SerializeField] private CargoCraneInventoryResolveStrategy inventoryResolveStrategy;

    private Transform m_NearestLoot;
    private Collider[] m_Colliders = new Collider[50];

    [SerializeField] private LineRenderer cranePullLineRenderer;

    private void Update()
    {
        int hits = Physics.OverlapSphereNonAlloc(
            transform.position,
            fetchRadius, 
            m_Colliders,
            LayerMask.GetMask("Default"), 
            QueryTriggerInteraction.Ignore
        );
        
        if (hits == m_Colliders.Length)
            Debug.LogWarning("All cargo discovery slots were used!");

        float nearestLootDistance = float.MaxValue;
        Loot nearestLoot = null;
        
        foreach (Collider hitCol in m_Colliders)
        {
            if (!hitCol)
                continue;

            Loot loot = hitCol.GetComponent<Loot>();
            if (!loot)
                continue;

            float distance = (loot.transform.position - transform.position).magnitude;
            if (distance < grabRadius)
            {
                Grab(loot);
            }
            else if (distance < fetchRadius && distance < nearestLootDistance)
            {
                nearestLootDistance = distance;
                nearestLoot = loot;
            }
        }
        
        if (nearestLoot != null)
        {
            if (cranePullLineRenderer && !cranePullLineRenderer.enabled)
                cranePullLineRenderer.enabled = true;
            if (!pullEventEmitter.IsPlaying())
                pullEventEmitter.Play();
            Pull(nearestLoot.transform);
        }
        else
        {
            if (cranePullLineRenderer && cranePullLineRenderer.enabled)
                cranePullLineRenderer.enabled = false;
            if (pullEventEmitter.IsPlaying())
                pullEventEmitter.Stop();
        }
    }

    private void Pull(Transform t)
    {
        Vector3 lookVector = (transform.position - t.position).normalized;
        t.position += lookVector * fetchSpeed * Time.deltaTime;
        RenderLine(t);
    }

    private void Grab(Loot loot)
    {
        print("Grab");
        grabEventEmitter.Play();
        EventLibrary.onCraneGrab.Invoke(loot.gameObject);
        Destroy(loot.gameObject);
    }

    private void RenderLine(Transform t)
    {
        if (!cranePullLineRenderer || !cranePullLineRenderer.enabled)
            return;

        cranePullLineRenderer.SetPosition(0, transform.position);
        cranePullLineRenderer.SetPosition(1, t.position);
    }
}