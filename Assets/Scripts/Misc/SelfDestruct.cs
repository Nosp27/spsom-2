using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float ttl= 1f;
    
    void Start()
    {
        Destroy(gameObject, ttl);
    }
}
