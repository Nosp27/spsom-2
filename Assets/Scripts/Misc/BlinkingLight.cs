using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    public float[] switches;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        Light light = GetComponent<Light>();
        int i = 0;
        while (true)
        {
            yield return new WaitForSeconds(switches[i % switches.Length]);
            light.enabled = !light.enabled;
            i++;
        }
    }
}
