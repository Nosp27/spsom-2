using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageStation : MonoBehaviour
{
    [SerializeField] private GameObject test_SpawnPrefab;
    
    private bool spawnLock = false;

    private bool doorOpened = true;
    private bool doorMoving = false;

    [SerializeField] private Collider garageTrigger;
    [SerializeField] private Transform door;
    [SerializeField] private Transform spawnPlace;

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
        garageTrigger.isTrigger = true;
        GameObject ship = Instantiate(shipPrefab, spawnPlace.position, spawnPlace.rotation);
        ship.GetComponent<Ship>().enabled = false;
        yield return MoveDoor(true);
        ship.GetComponent<Ship>().enabled = true;
        yield return EnsureGarageEmpty();
        garageTrigger.isTrigger = false;
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

    private IEnumerator EnsureGarageEmpty()
    {
        Collider[] cols = new Collider[100];
        while (true)
        {
            int numCols = Physics.OverlapBoxNonAlloc(
                garageTrigger.transform.position,
                garageTrigger.bounds.extents,
                cols,
                garageTrigger.transform.rotation
            );
            
            if (numCols == 0)  // Garage empty
            {
                yield break;
            }
            
            for (int i = 0; i < numCols; i++)
            {
                Collider col = cols[i];
                if (col != null && col.GetComponentInParent<Ship>() == null && !col.transform.IsChildOf(transform))
                {
                    Destroy(col.gameObject);
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }
}