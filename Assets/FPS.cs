using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private int fps;
    void Update()
    {
        Application.targetFrameRate = fps;
    }
}
