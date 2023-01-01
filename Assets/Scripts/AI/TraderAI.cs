using System.Collections.Generic;
using AI;
using UnityEngine;

[RequireComponent(typeof(ShipAIControls))]
public class TraderAI : MonoBehaviour
{
    private ShipAIControls m_AI;
    private Ship m_ThisShip => m_AI.thisShip;

    [SerializeField] private List<Transform> Places;

    private int CurrentPlaceIndex;
    private Transform CurrentPlace => Places[CurrentPlaceIndex % Places.Count];

    private void Start()
    {
        CurrentPlaceIndex = 0;
        m_AI = GetComponent<ShipAIControls>();
    }

    private void Update()
    {
        if (!m_ThisShip.Alive)
        {
            if (m_ThisShip.IsMoving())
                m_ThisShip.CancelMovement();
            return;
        }
        
        if (ShipAt(CurrentPlace))
        {
            CurrentPlaceIndex++;
        }
        
        m_AI.MoveAt(CurrentPlace.position);
    }

    bool ShipAt(Transform place)
    {
        return (m_ThisShip.transform.position - place.transform.position).magnitude < 20 && !m_ThisShip.IsMoving();
    }
}
