using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundControl : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        while (false)
        {
            bool danger = false;
            if(danger)
            {
                mixer.FindSnapshot("Danger").TransitionTo(2f);
            }
            else
            {
                mixer.FindSnapshot("Calm").TransitionTo(2f);
            }
            yield return new WaitForSeconds(0.8f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
