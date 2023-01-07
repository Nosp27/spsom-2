using UnityEngine;

public class TriggerOccupancy : MonoBehaviour
{
    public bool isOccupied;
    private BoxCollider refCollider;

    private Collider[] cols;

    private void Awake()
    {
        cols = new Collider[30];
    }

    private void Start()
    {
        refCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
        int n = Physics.OverlapBoxNonAlloc(transform.position, refCollider.size/2, cols, transform.rotation);
        bool tmpOccupied = false;
        for (int i = 0; i < n; i++)
        {
            Collider col = cols[i];
            if (CheckCollision(col))
            {
                tmpOccupied = true;
            }
        }

        isOccupied = tmpOccupied;
    }
    
    bool CheckCollision(Collider other)
    {
        return other != null && !other.isTrigger && !other.transform.IsChildOf(transform);
    }
}
