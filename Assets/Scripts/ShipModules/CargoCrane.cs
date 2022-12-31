using UnityEngine;

public enum CargoCraneInventoryResolveStrategy
{
    PLAYER_INVENTORY,
    ATTACHED_INVENTORY,
}

public class CargoCrane : MonoBehaviour
{
    [SerializeField] private float fetchSpeed = 10;
    [SerializeField] private float fetchRadius;
    [SerializeField] private float grabRadius;
    [SerializeField] private CargoCraneInventoryResolveStrategy inventoryResolveStrategy;
    
    private Transform m_NearestLoot;
    private InventoryController m_AttachedInventory;
    private Collider[] m_Colliders = new Collider[50];

    private void Start()
    {
        m_AttachedInventory = ResolveInventory();
    }

    private void Update()
    {
        int hits = Physics.OverlapSphereNonAlloc(transform.position, fetchRadius, m_Colliders);
        if (hits == m_Colliders.Length)
            Debug.LogWarning("All cargo discovery slots were used!");
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
                Destroy(loot.gameObject);
            } else if (distance < fetchRadius)
            {
                Pull(loot.transform);
            }
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
    }
    
    private void Grab(GameObject prefab)
    {
        if (!m_AttachedInventory)
            return;

        GameObject inventoryItemInstance = Instantiate(prefab, m_AttachedInventory.transform);
        m_AttachedInventory.PutItem(inventoryItemInstance.GetComponent<InventoryItem>());
        inventoryItemInstance.SetActive(false);
    }
}
