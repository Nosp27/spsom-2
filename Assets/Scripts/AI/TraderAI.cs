using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class TraderAI : MonoBehaviour
{
    private CollisionAvoidance CA;
    private Vector3 AvoidPoint;

    private Ship ThisShip;

    [SerializeField] private List<Transform> Places;

    private int CurrentPlaceIndex;
    private Transform CurrentPlace => Places[CurrentPlaceIndex % Places.Count];

    private void Start()
    {
        CurrentPlaceIndex = 0;
        CA = GetComponent<CollisionAvoidance>();
        ThisShip = GetComponent<Ship>();
    }

    private void Update()
    {
        if (ShipAt(CurrentPlace))
        {
            CurrentPlaceIndex++;
        }
        
        MoveAt(CurrentPlace.position);
    }

    bool ShipAt(Transform place)
    {
        return (ThisShip.transform.position - place.transform.position).magnitude < 20 && !ThisShip.IsMoving();
    }

    void MoveAt(Vector3 point)
    {
        if (point != Vector3.zero)
        {
            AvoidPoint = CA.AvoidPoint(point);
            Debug.DrawLine(transform.position, AvoidPoint, Color.yellow);
        }
        
        if (!ThisShip.IsMoving() && AvoidPoint != Vector3.zero) {
            AvoidPoint = Vector3.zero;
        }

        if (AvoidPoint == Vector3.zero)
        {
            ThisShip.Move(point);
        }
        else
        {
            ThisShip.Move(AvoidPoint);
        }
    }
}
