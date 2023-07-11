using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : MonoBehaviour
{
    private GameObject lootPrefab;
    public void Die()
    {
        if (lootPrefab)
        {
            Instantiate(lootPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning($"{name} is Prey but has no loot");
        }
    }
}
