using System;
using UnityEngine;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    private Func<Collider, bool> m_Criteria;
    public UnityEvent<Collider> onTriggerEnter;
    public UnityEvent<Collider> onTriggerExit;
    
    public static bool CRITERIA_PLAYER_SHIP(Collider col)
    {
        Ship ship = col.GetComponentInParent<Ship>();
        if (!ship)
        {
            return false;
        }

        return ship.isPlayerShip;
    }
    
    public static bool CRITERIA_EVERYTHING(Collider col)
    {
        return true;
    }
    
    public static bool CRITERIA_ANY_SHIP(Collider col)
    {
        return col.GetComponentInParent<Ship>() != null;
    }

    private void Awake()
    {
        m_Criteria = CRITERIA_PLAYER_SHIP;
        onTriggerEnter = new UnityEvent<Collider>();
        onTriggerExit = new UnityEvent<Collider>();
    }

    public void SetCriteria(Func<Collider, bool> passForCollider)
    {
        m_Criteria = passForCollider;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Criteria.Invoke(other))
        {
            onTriggerEnter.Invoke(other);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (m_Criteria.Invoke(other))
        {
            onTriggerExit.Invoke(other);
        }
    }
}
