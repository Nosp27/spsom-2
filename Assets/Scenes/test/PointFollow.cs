using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SpaceShip.ShipServices;
using UnityEngine;

public class PointFollow : MonoBehaviour
{
    [SerializeField] private Transform waypointGroup;
    [SerializeField] private MovementConfig config;
    private Transform[] waypoints;
    private ShipMovementService ms;
    private int i = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        ms = GetComponent<ShipMovementService>();
        ms.Init(transform, config);
        waypoints = Enumerable.Range(0, waypointGroup.transform.childCount).Select(x => waypointGroup.transform.GetChild(x)).ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, waypoints[i].transform.position) < 5f)
        {
            i++;
            i %= waypoints.Length;
        }
        ms.Move(waypoints[i].position, 1);
    }

    private void FixedUpdate()
    {
        ms.Tick();
    }
}
