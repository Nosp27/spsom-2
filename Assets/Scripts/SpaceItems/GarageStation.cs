using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageStation : MonoBehaviour
{
    [SerializeField] private GameObject test_SpawnPrefab;

    private bool spawnLock = false;

    private bool doorOpened = false;
    private bool doorMoving = false;

    [SerializeField] private TriggerOccupancy garageTrigger;
    [SerializeField] private Transform door;
    [SerializeField] private Transform spawnPlace;
    [SerializeField] private Transform dropoffPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            StartCoroutine(MoveDoor(false));

        if (Input.GetKeyDown(KeyCode.V))
            StartCoroutine(MoveDoor(true));

        if (Input.GetKeyDown(KeyCode.L))
            StartCoroutine(SpawnShip(test_SpawnPrefab));
    }

    private IEnumerator SpawnShip(GameObject shipPrefab)
    {
        if (spawnLock)
        {
            yield break;
        }
        
        spawnLock = true;
        yield return new WaitUntil(() => !garageTrigger.isOccupied);
        GameObject ship = Instantiate(shipPrefab, spawnPlace.position, spawnPlace.rotation);
        ship.GetComponent<Ship>().enabled = false;
        yield return MoveDoor(true);
        yield return TransferShipToDropoffPoint(ship.transform);
        ship.GetComponent<Ship>().enabled = true;
        yield return new WaitUntil(() => !garageTrigger.isOccupied);
        yield return MoveDoor(false);
        spawnLock = false;
    }

    private IEnumerator MoveDoor(bool open)
    {
        if (doorOpened == open && !doorMoving)
            yield break;

        if (doorOpened != open && doorMoving)
            yield break;

        doorMoving = true;
        Vector3 initialRotationEuler = door.localRotation.eulerAngles;
        float rotationStep;
        float finalAngle;
        Predicate<Transform> rotationCondition;
        if (open)
        {
            rotationStep = 2f;
            finalAngle = 90;
            rotationCondition = d => d.localRotation.eulerAngles.x < finalAngle;
        }
        else
        {
            rotationStep = -2f;
            finalAngle = 0;
            rotationCondition = d => d.localRotation.eulerAngles.x > finalAngle && d.localRotation.eulerAngles.x < 91;
        }

        while (rotationCondition.Invoke(door))
        {
            door.localRotation *= Quaternion.Euler(rotationStep, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }

        door.localRotation = Quaternion.Euler(finalAngle, initialRotationEuler.y, initialRotationEuler.z);
        doorMoving = false;
        doorOpened = open;
    }

    IEnumerator TransferShipToDropoffPoint(Transform ship)
    {
        Vector3 way;
        Ship shipComp = ship.GetComponent<Ship>();
        do
        {
            float deliverySpeed = shipComp.LinearSpeed;
            way = (dropoffPoint.position - ship.position);
            ship.position += way.normalized * deliverySpeed * Time.deltaTime;
            yield return new WaitForSeconds(0);
        } while (way.magnitude > 1f);
    }
}