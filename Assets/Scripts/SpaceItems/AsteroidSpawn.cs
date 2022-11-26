using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AsteroidSpawn : MonoBehaviour
{
    public GameObject[] prefabs;
    public float SpawnDelay;
    private float FieldSize;

    Vector3 randomPosition(float Size)
    {
        Vector3 v = Random.onUnitSphere * Size;
        v.y = 0;
        return v;
    }
    
    IEnumerator Start()
    {
        FieldSize = GetComponent<SphereCollider>().radius;
        while (true)
        {
            Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform.position + randomPosition(FieldSize),
                transform.rotation * Quaternion.AngleAxis(45f*(Random.value * 2 - 1), Vector3.up));
            yield return new WaitForSeconds(SpawnDelay + Random.value * SpawnDelay * 0.5f);
        }
    }
}
