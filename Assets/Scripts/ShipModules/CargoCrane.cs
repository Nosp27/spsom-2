using System;
using FMODUnity;
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
    private InventoryController m_AttachedInventory;
    private Collider[] m_Colliders = new Collider[50];

    [SerializeField] private LineRenderer cranePullLineRenderer;

    public UnityEvent<GameObject> onGrab { get; private set; }

    private void Awake()
    {
        onGrab = new UnityEvent<GameObject>();
    }

    private void Start()
    {
        m_AttachedInventory = ResolveInventory();
    }

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
                Grab(loot.GetLootPrefab());
                onGrab.Invoke(loot.gameObject);
                Destroy(loot.gameObject);
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

    private InventoryController ResolveInventory()
    {
        if (inventoryResolveStrategy == CargoCraneInventoryResolveStrategy.ATTACHED_INVENTORY)
            return GetComponent<InventoryController>();

        return GameController.Current.Inventory;
    }

    private void Pull(Transform t)
    {
        Vector3 lookVector = (transform.position - t.position).normalized;
        t.position += lookVector * fetchSpeed * Time.deltaTime;
        RenderLine(t);
    }

    private void Grab(GameObject prefab)
    {
        if (!m_AttachedInventory)
            return;
        
        grabEventEmitter.Play();
        
        GameObject inventoryItemInstance = Instantiate(prefab, m_AttachedInventory.transform);
        m_AttachedInventory.PutItem(inventoryItemInstance.GetComponent<InventoryItem>());
        inventoryItemInstance.SetActive(false);
    }

    private void RenderLine(Transform t)
    {
        if (!cranePullLineRenderer || !cranePullLineRenderer.enabled)
            return;

        cranePullLineRenderer.SetPosition(0, transform.position);
        cranePullLineRenderer.SetPosition(1, t.position);
    }
}