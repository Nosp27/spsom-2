using System.Collections.Generic;
using UnityEngine;

public class RandomObjectEnable : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameObjects;

    void Awake()
    {
        int i = 0;
        int idx = Random.Range(0, gameObjects.Count);
        foreach (var go in gameObjects)
            go.SetActive(i++ == idx);
    }
}
