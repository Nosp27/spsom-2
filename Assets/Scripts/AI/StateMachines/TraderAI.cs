using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using UnityEngine;

[RequireComponent(typeof(ShipAIControls))]
public class TraderAI : MonoBehaviour
{
    private ShipAIControls m_AI;
    private Ship m_ThisShip => m_AI.thisShip;

    [SerializeField] private List<Transform> Places;


    private bool isWorking = true;
    private int CurrentPlaceIndex;
    private Transform CurrentPlace => Places.Count > 0 ? Places[CurrentPlaceIndex % Places.Count] : null;

    private void Awake()
    {
        if (Places == null || Places.Count == 0 || Places[0] == null)
        {
            Places = GameObject.FindGameObjectsWithTag("Waypoint").Select(x => x.transform).ToList();
        }
    }

    private void Start()
    {
        CurrentPlaceIndex = 0;
        m_AI = GetComponent<ShipAIControls>();
    }

    private void Update()
    {
        if (!m_AI.enabled)
            return;
        
        if (!m_ThisShip.Alive)
        {
            if (m_ThisShip.IsMoving())
                m_ThisShip.CancelMovement();
            return;
        }
        
        if (CurrentPlace == null)
            return;
        
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
