using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BorderZone : MonoBehaviour
{
    [SerializeField] private Vector3 swingExtent;
    [SerializeField] private float swingSpeed = 5;


    private Transform[] beacons;
    private Vector3[] initialPositions;
    private float[] beaconSwingOffsets;
    [SerializeField] private LineRenderer lr;
    
    private Func<float, float> swingFunc = x => Mathf.Sin(x);

    private void Start()
    {
        beacons = new Transform[transform.childCount];
        lr.positionCount = transform.childCount;
        lr.useWorldSpace = true;
        
        beaconSwingOffsets = new float[transform.childCount];
        initialPositions = new Vector3[transform.childCount];

        for (int i = 0; i < beacons.Length; ++i)
        {
            beacons[i] = transform.GetChild(i);
            initialPositions[i] = beacons[i].position;
            beaconSwingOffsets[i] = Random.value;
        }
    }

    private void Update()
    {
        for (int i = 0; i < beacons.Length; ++i)
        {
            beacons[i].position = initialPositions[i] + swingExtent * swingFunc.Invoke(swingSpeed * Time.time + beaconSwingOffsets[i]);
            lr.SetPosition(i, beacons[i].position);
        }
    }
}