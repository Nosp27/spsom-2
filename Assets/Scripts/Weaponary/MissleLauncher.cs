using System;
using System.Collections;
using System.Collections.Generic;
using GameControl.StateProcessors;
using UnityEngine;

public class MissleLauncher : MonoBehaviour
{
    [SerializeField] private GameObject MissilePrefab;
    private MovementSP msp;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (msp == null)
        {
            msp = GameController.Current.GetComponentInChildren<MovementSP>();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            LunchMissile();
        }
    }

    void LunchMissile()
    {
        GameObject missile = Instantiate(MissilePrefab, transform.position, transform.rotation);
        Guided g = missile.GetComponent<Guided>();
        Explosive e = missile.GetComponent<Explosive>();

        Transform lockTarget = msp.LockTarget?.transform;
        if (g && lockTarget)
            g.Target = lockTarget;

        if (e)
        {
            if(lockTarget)
                e.DetonateForDistance(msp.LockTarget.transform, 2);
            e.DetonateForTime(10);
        }
        
        if (audioSource != null)
            audioSource.Play();
    }
}
