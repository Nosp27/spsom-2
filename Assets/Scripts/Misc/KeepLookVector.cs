using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepLookVector : MonoBehaviour
{
    [SerializeField] private Vector3 lookVector;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(lookVector);
    }
}
