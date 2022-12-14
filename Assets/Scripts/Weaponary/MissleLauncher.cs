using GameControl.StateMachine.GameControlStates;
using UnityEngine;

public class MissleLauncher : MonoBehaviour
{
    [SerializeField] private GameObject MissilePrefab;
    private Movement msp;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (msp == null)
        {
            msp = GameController.Current.GetComponentInChildren<Movement>();
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

        Transform lockTarget = msp.lockTarget?.transform;
        if (g && lockTarget)
            g.Target = lockTarget;

        if (e)
        {
            if(lockTarget)
                e.DetonateForDistance(msp.lockTarget.transform, 2);
            e.DetonateForTime(10);
        }
        
        if (audioSource != null)
            audioSource.Play();
    }
}
