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

        while (true)
        {
            FighterAI[] enemyShips = FindObjectsOfType<FighterAI>();
            print($"Found {enemyShips.Length} AIs");

            bool danger = false;
            
            foreach (var ai in enemyShips)
            {
                if (ai.Enemy && ai.Enemy.isPlayerShip && ai.ThisShip.Alive)
                {
                    danger = true;
                    break;
                }
            }
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
