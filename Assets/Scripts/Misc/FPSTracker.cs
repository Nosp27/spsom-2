using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FPSTracker : MonoBehaviour
{
    Text textbox;
    // Start is called before the first frame update
    void Start()
    {
        textbox = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        textbox.text = $"{(int)(1f / Time.deltaTime)} fps";
    }
}
